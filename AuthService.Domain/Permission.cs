using AuthService.Domain.Common;

namespace AuthService.Domain
{
    public class Permission : AuditableEntity
    {
        public string Name { get; private set; }           // Ej: "CanEditOrders"
        public string Description { get; private set; }    // Opcional, explicación
        public bool IsActive { get; private set; }         // Permiso activo o no

        // Navigation: Roles that have this permission
        private readonly List<RolePermission> _rolePermissions = new();
        private readonly List<UserPermission> _userPermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();
        public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions.AsReadOnly();

        // Constructor privado para EF Core
        private Permission() { }

        // Constructor público para creación controlada
        public Permission(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Permission name is required.", nameof(name));

            Name = name;
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // Comportamiento de dominio
        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }        
    }
}
