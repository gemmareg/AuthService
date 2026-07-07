namespace AuthService.Application.Abstractions.Services
{
    /// <summary>
    /// Emite y cachea el access token de la cuenta de servicio (p. ej. "EventPublisher") que
    /// AuthService usa para autenticarse a sí mismo contra otros servicios (MessageBrokerService).
    /// </summary>
    public interface IServiceTokenProvider
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
