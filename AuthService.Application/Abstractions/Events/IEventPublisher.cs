using AuthService.Application.Dtos.Events;

namespace AuthService.Application.Abstractions.Events
{
    public interface IEventPublisher
    {
        void PublishUserRegistered(UserRegisteredEvent evt);
    }
}
