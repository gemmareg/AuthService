using System.Net.Http.Headers;
using AuthService.Application.Abstractions.Services;

namespace AuthService.Host.Events
{
    /// <summary>Adjunta el access token de la cuenta de servicio EventPublisher a cada llamada a MessageBrokerService.</summary>
    public class MessageBrokerAuthHandler : DelegatingHandler
    {
        private readonly IServiceTokenProvider _tokenProvider;

        public MessageBrokerAuthHandler(IServiceTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
