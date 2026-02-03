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
        private readonly List<UserRole> _userRoles = [];
        private readonly List<UserPermission> _userPermissions = [];
        private readonly List<Token> _tokens = [];
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
        public IReadOnlyCollection<UserPermission> UserClaims => _userPermissions.AsReadOnly();
        public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();

        private User() { }

        private User(Guid id, string username, string email, string passwordHash, string? name, string? surname)
        {
            Id = id;
            Username = username;
            Email = email;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            Name = name;
            Surname = surname;
        }

        public static Result<User> Create(string username, string email, string passwordHash, string? name, string? surname)
        {
            if (string.IsNullOrWhiteSpace(username))
                return Result<User>.Fail(ErrorMessages.USERNAME_NOT_NULL);
            if (string.IsNullOrWhiteSpace(email))
                return Result<User>.Fail("Email is required");
            if(string.IsNullOrWhiteSpace(passwordHash))
                return Result<User>.Fail("Password hash is required");

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

        public Result AssignRole(UserRole userRole)
        {
            if (_userRoles.Any(r => r.Id == userRole.Id))
                return Result.Fail(ErrorMessages.ROLE_ALREADY_ASSIGNED);

            _userRoles.Add(userRole);
            return Result.Ok();
        }

        public void RevokeRole(UserRole userRole)
        {
            _userRoles.Remove(userRole);
        }

        public Result AddPermissions(Permission permission)
        {
            if (permission == null) return Result.Fail(ErrorMessages.CLAIM_NOT_NULL);
            
            if (_userPermissions.Any(c => c.Id == permission.Id))
                return Result.Fail(ErrorMessages.CLAIM_ALREADY_ASSIGNED);
            var userPermission = UserPermission.Create(this.Id, permission.Id);
            _userPermissions.Add(userPermission);
            return Result.Ok();
        }

        public void RemovePermission(UserPermission userPermission)
        {
            _userPermissions.Remove(userPermission);
        }
    }
}
