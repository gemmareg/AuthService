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
        private readonly List<UserRole> _userRoles = new();
        private readonly List<RolePermission> _roleClaims = new();

        public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
        public IReadOnlyCollection<RolePermission> RoleClaims => _roleClaims;

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

        public Result AddRoleClaim(RolePermission roleClaim)
        {
            if (roleClaim == null) return Result.Fail(ErrorMessages.CLAIM_NOT_NULL);
            if (_roleClaims.Any(c => c.Id == roleClaim.Id))
                return Result.Fail(ErrorMessages.CLAIM_ALREADY_ASSIGNED);

            _roleClaims.Add(roleClaim);
            return Result.Ok();
        }
    }
}
