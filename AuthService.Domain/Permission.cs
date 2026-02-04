using AuthService.Domain.Common;
using AuthService.Shared;
using AuthService.Shared.Result.Generic;

namespace AuthService.Domain
{
    public class Permission : AuditableEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation: Roles that have this permission
        private readonly List<Role> _roles = new();
        private readonly List<User> _users = new();
        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        // Constructor privado para EF Core
        private Permission() { }

        // Constructor público para creación controlada
        public Permission(string name, string description)
        {
            Name = name;
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Permission> Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Permission>.Fail(ErrorMessages.TYPE_PERMISSION_NOT_NULL);

            return Result<Permission>.Ok(new Permission(name, description));
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
