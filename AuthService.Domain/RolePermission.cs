using AuthService.Domain.Common;

namespace AuthService.Domain
{
    public class RolePermission : AuditableEntity
    {
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = null!;
        public Guid PermissionId { get; private set; }
        public Permission Permission { get; private set; } = null!;

        private RolePermission() { }

        private RolePermission(Role role, Permission permission)
        {
            Role = role;
            Permission = permission;
            RoleId = role.Id;
            PermissionId = permission.Id;
        }

        public static RolePermission Create(Role role, Permission permission) => new RolePermission(role, permission);
    }
}
