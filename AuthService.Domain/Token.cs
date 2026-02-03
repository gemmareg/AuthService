using AuthService.Domain.Abstractions;
using AuthService.Domain.Common;
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

        public Token(Guid userId, TokenType type, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Type = type;
            ExpiresAt = expiresAt;
        }

        public void Revoke()
        {
            IsRevoked = true;
        }
    }
}
