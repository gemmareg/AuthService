using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Extensions.Options;
using AuthService.Domain.Policies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static AuthService.Shared.Enums;

namespace AuthService.Application.Services
{
    public class EventPublisherTokenProvider : IServiceTokenProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EventPublisherOptions _options;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private string? _cachedToken;
        private DateTime _cachedTokenExpiresAt = DateTime.MinValue;

        public EventPublisherTokenProvider(IServiceScopeFactory scopeFactory, IOptions<EventPublisherOptions> options)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (_cachedToken is not null && DateTime.UtcNow < _cachedTokenExpiresAt)
                return _cachedToken;

            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_cachedToken is not null && DateTime.UtcNow < _cachedTokenExpiresAt)
                    return _cachedToken;

                using var scope = _scopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var tokenGenerator = scope.ServiceProvider.GetRequiredService<ITokenGenerator>();

                var user = await userRepository.GetByEmailWithRolesAsync(_options.Email)
                    ?? throw new InvalidOperationException($"Event publisher service account '{_options.Email}' was not found.");

                _cachedToken = await tokenGenerator.GenerateAccessTokenAsync(user);

                // Renovar antes de que expire de verdad, para no arriesgarnos a usar
                // un token caducado en el instante siguiente a devolverlo.
                var accessTokenLifetime = TokenPolicies.GetExpiration(TokenType.Access);
                _cachedTokenExpiresAt = DateTime.UtcNow.Add(accessTokenLifetime * 0.8);

                return _cachedToken;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
