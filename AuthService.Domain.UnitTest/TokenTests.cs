using static AuthService.Shared.Enums;

namespace AuthService.Domain.UnitTest
{
    public class TokenTests
    {
        [Fact]
        public void Revoke_Should_Mark_Token_As_Revoked()
        {
            var token = Token.Create(
                Guid.NewGuid(),
                TokenType.Access
            ).Data!;

            token.Revoke();

            Assert.True(token.IsRevoked);
        }

        [Fact]
        public void Create_Should_Create_Valid_Token()
        {
            var token = Token.Create(Guid.NewGuid(), TokenType.Access).Data!;

            Assert.False(token.IsExpired);
            Assert.True(token.IsValid);
        }

        [Fact]
        public void Revoked_Token_Should_Not_Be_Valid()
        {
            var token = Token.Create(Guid.NewGuid(), TokenType.Access).Data!;

            token.Revoke();

            Assert.False(token.IsValid);
        }

        [Fact]
        public void Expired_Token_Should_Not_Be_Valid()
        {
            var token = Token.Create(Guid.NewGuid(), TokenType.Access).Data!;

            typeof(Token)
                .GetProperty(nameof(Token.ExpiresAt))!
                .SetValue(token, DateTime.UtcNow.AddMinutes(-1));

            Assert.True(token.IsExpired);
            Assert.False(token.IsValid);
        }

        [Fact]
        public void Create_With_Invalid_TokenType_Should_Fail()
        {
            var result = Token.Create(Guid.NewGuid(), (TokenType)999);

            Assert.False(result.Success);
        }

        [Fact]
        public void Create_With_Empty_UserId_Should_Fail()
        {
            var result = Token.Create(Guid.Empty, TokenType.Access);

            Assert.False(result.Success);
        }
    }
}
