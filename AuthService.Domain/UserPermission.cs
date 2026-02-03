using AuthService.Domain.Common;

namespace AuthService.Domain
{
    public class UserPermission : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public Guid PermissionId { get; private set; }

        public User User { get; private set; }
        public Permission Permission { get; private set; }

        private UserPermission() { }

        private UserPermission(Guid userId, Guid permissionId)
        {
            UserId = userId;
            PermissionId = permissionId;
        }

        public static UserPermission Create(Guid userId, Guid permissionId) => new(userId, permissionId);
    }
}
