using AuthService.Domain.Common;
using AuthService.Domain.Policies;
using AuthService.Shared.Result.Generic;
using static AuthService.Shared.Enums;

namespace AuthService.Domain
{
    public class Token : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public TokenType Type { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public User? User { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid => !IsExpired && !IsRevoked;

        private Token(Guid userId, TokenType type)
        {
            UserId = userId;
            Type = type;
            ExpiresAt = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(type));
        }

        public static Result<Token> Create(Guid userId, TokenType type)
        {
            if (userId == Guid.Empty)
                return Result<Token>.Fail("UserId is required");

            if (!Enum.IsDefined(typeof(TokenType), type))
                return Result<Token>.Fail("Invalid token type");

            return Result<Token>.Ok(new Token(userId, type));
        }

        public void Revoke()
        {
            IsRevoked = true;
        }
    }
}
