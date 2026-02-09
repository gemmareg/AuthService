using AuthService.Domain.Common;
using AuthService.Shared;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;

namespace AuthService.Domain
{
    public class User : AuditableEntity
    {
        public string Username { get; private set; }
        public string? Name { get; private set; }
        public string? Surname { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        private readonly List<Role> _roles = [];
        private readonly List<Permission> _permissions = [];
        private readonly List<Token> _tokens = [];
        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();
        public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();

        private User() { }

        private User(Guid id, string username, string email, string passwordHash, string? name, string? surname)
        {
            Id = id;
            Username = username;
            Email = email;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            PasswordHash = passwordHash;
            Name = name;
            Surname = surname;
        }

        public static Result<User> Create(string username, string email, string passwordHash, string name, string surname)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Result<User>.Fail(ErrorMessages.USERNAME_NOT_NULL);
            if (string.IsNullOrWhiteSpace(email))
                return Result<User>.Fail(ErrorMessages.EMAIL_NOT_NULL);
            if(string.IsNullOrWhiteSpace(passwordHash))
                return Result<User>.Fail("Password hash is required");
            if(string.IsNullOrWhiteSpace(name))
                return Result<User>.Fail("Name is required");
            if (string.IsNullOrWhiteSpace(surname))
                return Result<User>.Fail("Surname is required");

            var user = new User(Guid.NewGuid(), username, email, passwordHash, name, surname);

            return Result<User>.Ok(user);
        }

        public Result UpdateData(string? name, string? surname, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Fail("Email is required");
            Name = name;
            Surname = surname;
            Email = email;
            return Result.Ok();
        }

        public Result UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                return Result.Fail("New password hash is required");
            PasswordHash = newPasswordHash;
            return Result.Ok();
        }

        public Result AssignRole(Role role)
        {
            if (role == null) return Result.Fail(ErrorMessages.ROLE_NOT_NULL);
            if (_roles.Any(r => r.Id == role.Id))
                return Result.Fail(ErrorMessages.ROLE_ALREADY_ASSIGNED);

            _roles.Add(role);
            return Result.Ok();
        }

        public Result RevokeRole(Role role)
        {
            if(!_roles.Any(r => r.Id == role.Id))
                return Result.Fail("Role doesn't exist in user");

            _roles.Remove(role);
            return Result.Ok();
        }

        public Result AddPermissions(Permission permission)
        {
            if (permission == null) return Result.Fail(ErrorMessages.PERMISSION_NOT_NULL);
            
            if (_permissions.Any(c => c.Id == permission.Id))
                return Result.Fail(ErrorMessages.PERMISSION_ALREADY_ASSIGNED);

            _permissions.Add(permission);
            return Result.Ok();
        }

        public Result RemovePermission(Permission permission)
        {
            if (permission == null) return Result.Fail(ErrorMessages.PERMISSION_NOT_NULL);
            if (!_permissions.Any(p => p.Id == permission.Id)) 
                return Result.Fail(ErrorMessages.PERMISSION_DOESNT_EXIST_IN_USER);

            _permissions.Remove(permission);

            return Result.Ok();
        }
    }
}
