using AuthService.Domain.Common;

namespace AuthService.Domain
{
    public class UserRole : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        public User User { get; private set; }
        public Role Role { get; private set; }

        private UserRole() { }

        private UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public static UserRole Create(Guid userId, Guid roleId) => new(userId, roleId);
    }
}
