using Auth.Contracts.Events;

namespace AuthService.Application.Abstractions.Events
{
    public interface IEventPublisher
    {
        void PublishUserRegistered(UserRegisteredEvent evt);
        void PublishUserSoftDeleted(UserSoftDeletedEvent evt);
        void PublishUserActivated(UserActivatedEvent evt);
        void PublishEmailChanged(EmailChangedEvent evt);
        void PublishAdminCreated(AdminCreatedEvent evt);
    }
}
