using AuthService.Domain.Common;
using AuthService.Shared;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;

namespace AuthService.Domain
{
    public class Role : AuditableEntity
    {
        public string Name { get; private set; } = string.Empty;

        // Navigation properties
        private readonly List<User> _users = new();
        private readonly List<Permission> _permissions = new();

        public IReadOnlyCollection<User> Users => _users;
        public IReadOnlyCollection<Permission> Permissions => _permissions;

        private Role() { }

        private Role(string name)
        {
            Name = name;
        }

        public static Result<Role> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Result<Role>.Fail(ErrorMessages.ROLE_NAME_NOT_NULL);
            var role = new Role(name);

            return Result<Role>.Ok(role);
        }

        public Result AddPermission(Permission permission)
        {
            if (permission == null) return Result.Fail(ErrorMessages.PERMISSION_NOT_NULL);
            if (_permissions.Any(c => c.Id == permission.Id))
                return Result.Fail(ErrorMessages.PERMISSION_ALREADY_ASSIGNED);

            _permissions.Add(permission);
            return Result.Ok();
        }
    }
}
