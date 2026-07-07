using Auth.Contracts.Events;

namespace AuthService.Application.Abstractions.Events
{
    public interface IEventPublisher
    {
        Task PublishUserRegisteredAsync(UserRegisteredEvent evt);
        Task PublishUserSoftDeletedAsync(UserSoftDeletedEvent evt);
        Task PublishUserActivatedAsync(UserActivatedEvent evt);
        Task PublishEmailChangedAsync(EmailChangedEvent evt);
        Task PublishAdminCreatedAsync(AdminCreatedEvent evt);
    }
}
