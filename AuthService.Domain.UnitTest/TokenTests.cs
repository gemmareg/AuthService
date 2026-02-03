using static AuthService.Shared.Enums;

namespace AuthService.Domain.UnitTest
{
    public class TokenTests
    {
        [Fact]
        public void Revoke_Should_Mark_Token_As_Revoked()
        {
            var token = new Token(
                Guid.NewGuid(),
                TokenType.Access,
                DateTime.UtcNow.AddMinutes(10)
            );

            token.Revoke();

            Assert.True(token.IsRevoked);
        }
    }
}
