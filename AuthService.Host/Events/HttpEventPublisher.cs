using Auth.Contracts.Events;
using AuthService.Application.Abstractions.Events;
using MessageBroker.Client;

namespace AuthService.Host.Events
{
    /// <summary>Publica los eventos de dominio de AuthService en MessageBrokerService vía HTTP.</summary>
    public class HttpEventPublisher : IEventPublisher
    {
        private const string Source = "AuthService";

        private readonly IMessageBrokerClient _client;

        public HttpEventPublisher(IMessageBrokerClient client)
        {
            _client = client;
        }

        public Task PublishUserRegisteredAsync(UserRegisteredEvent evt) => PublishAsync("user.registered", evt);

        public Task PublishUserSoftDeletedAsync(UserSoftDeletedEvent evt) => PublishAsync("user.soft-deleted", evt);

        public Task PublishUserActivatedAsync(UserActivatedEvent evt) => PublishAsync("user.activated", evt);

        public Task PublishEmailChangedAsync(EmailChangedEvent evt) => PublishAsync("user.email-changed", evt);

        public Task PublishAdminCreatedAsync(AdminCreatedEvent evt) => PublishAsync("admin.created", evt);

        private async Task PublishAsync(string eventType, object payload)
            => await _client.PublishAsync(eventType, payload, Source);
    }
}
