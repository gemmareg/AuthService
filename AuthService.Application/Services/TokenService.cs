using Auth.Contracts;
using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Application.Dtos;
using AuthService.Application.Extensions.Options;
using AuthService.Domain;
using AuthService.Domain.Extensions;
using AuthService.Domain.Policies;
using AuthService.Shared.Result.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static AuthService.Shared.Enums;

namespace AuthService.Application.Services
{
    public class TokenService : ITokenGenerator, ITokenRefresher
    {
        private const string AdminRoleName = "Admin";

        private readonly JwtSettings _settings;
        private readonly ITokenRepository _tokenRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            ITokenRepository tokenRepository,
            IPermissionRepository permissionRepository,
            IOptions<JwtSettings> settings,
            IUnitOfWork unitOfWork,
            ILogger<TokenService> logger)
        {
            _settings = settings.Value;
            _tokenRepository = tokenRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> GenerateAccessTokenAsync(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));

            var permissionNames = await GetPermissionNamesForTokenAsync(user);
            claims.AddRange(permissionNames.Select(p => new Claim(AuthClaimTypes.Permission, p)));

            var expiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Access));

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            _logger.LogInformation("Generated access token for user {UserId} with email {Email}", user.Id, user.Email);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Para un usuario Admin, los claims de permiso incluyen TODOS los
        /// permisos activos del sistema, calculados en el momento de emitir
        /// el token — no hace falta mantener sincronizada ninguna asignación
        /// Admin-Permission en BD, y consumidores externos que validen el
        /// token mirando solo estos claims (sin conocer la convención de rol
        /// "Admin") ven exactamente lo que deben ver.
        /// </summary>
        private async Task<IEnumerable<string>> GetPermissionNamesForTokenAsync(User user)
        {
            var isAdmin = user.Roles.Any(r => r.Name.Equals(AdminRoleName, StringComparison.OrdinalIgnoreCase));
            if (!isAdmin)
            {
                return user.GetEffectivePermissions();
            }

            var allPermissions = await _permissionRepository.GetAllAsync();
            return allPermissions
                .Where(p => p.IsActive)
                .Select(p => p.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<Result<RefreshToken>> GenerateRefreshToken(Guid userId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var tokenValue = Convert.ToBase64String(randomBytes);
            var hash = HashToken(tokenValue);

            var tokenResult = Token.Create(userId, TokenType.Refresh, hash);

            if (!tokenResult.Success) return Result<RefreshToken>.Fail(tokenResult.Message);

            var domainToken = tokenResult.Data!;
            await _tokenRepository.AddAsync(domainToken);

            _logger.LogInformation("Generated refresh token for user {UserId}", userId);

            return Result<RefreshToken>.Ok(new() { Token = tokenValue, Expiration = domainToken.ExpiresAt });
        }
        
        public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            var tokenHash = HashToken(refreshToken);
            var token = await _tokenRepository.GetByTokenHashAsync(tokenHash);

            if (token is null || token.IsExpired || token.IsRevoked)
            {
                _logger.LogWarning("Invalid refresh token attempt");
                return Result<AuthResponse>.Fail("Invalid refresh token");
            }

            var user = token.User;

            if (!user.IsActive)
            {
                _logger.LogWarning("Attempt to refresh token for inactive user {UserId}", user.Id);
                return Result<AuthResponse>.Fail("User account is inactive");
            }

            var newAccessToken = await GenerateAccessTokenAsync(user);

            var newRefreshToken = await GenerateRefreshToken(user.Id);
            token.Revoke();

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Refreshed tokens for user {UserId}", user.Id);

            return Result<AuthResponse>.Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Data!.Token,
                RefreshTokenExpiration = newRefreshToken.Data.Expiration
            });
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
