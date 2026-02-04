using static AuthService.Shared.Enums;

namespace AuthService.Domain.Policies
{
    public static class TokenPolicies
    {
        public static TimeSpan GetExpiration(TokenType type) =>
        type switch
        {
            TokenType.Access => TimeSpan.FromMinutes(15),
            TokenType.Refresh => TimeSpan.FromDays(7),
            TokenType.EmailConfirmation => TimeSpan.FromHours(24),
            TokenType.PasswordReset => TimeSpan.FromMinutes(30),
            _ => TimeSpan.FromMinutes(0)
        };
    }
}
