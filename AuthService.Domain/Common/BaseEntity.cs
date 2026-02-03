using AuthService.Domain.Abstractions;

namespace AuthService.Domain.Common
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }
}
