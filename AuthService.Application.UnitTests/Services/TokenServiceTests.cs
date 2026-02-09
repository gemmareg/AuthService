using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Extensions.Options;
using AuthService.Application.Services;
using AuthService.Domain;
using AuthService.Domain.Policies;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static AuthService.Shared.Enums;

namespace AuthService.Application.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private JwtSettings JwtSettingsFixture => new()
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "ThisIsASecretKeyForTestingPurposesOnly"
        };

        public TokenServiceTests()
        {
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            var options = Options.Create(JwtSettingsFixture);
            _tokenService = new TokenService(_tokenRepositoryMock.Object, options);
        }

        [Fact]
        public void GenerateAccessToken_ShouldContainClaimsAndCorrectExpiration()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = "test@test.com";
            var roles = new List<string> { "admin", "user" };

            // Act
            var tokenString = _tokenService.GenerateAccessToken(userId, email, roles);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(tokenString));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenString);

            // Verificar claims
            Assert.Equal(userId.ToString(), jwt.Subject);
            Assert.Equal(email, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            foreach (var role in roles)
            {
                Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == role);
            }

            // Verificar expiración (aproximado, porque DateTime.UtcNow puede variar)
            var expectedExpiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Access));
            var difference = (jwt.ValidTo - expectedExpiration).TotalSeconds;
            Assert.True(Math.Abs(difference) < 5, "AccessToken expiration is not correct");
        }

        [Fact]
        public async Task GenerateRefreshToken_ShouldContainClaimsAndCorrectExpiration()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _tokenRepositoryMock.Reset(); // limpiar invocaciones previas

            // Mock para AddAsync simplemente devuelve completed task
            _tokenRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Token>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.GenerateRefreshToken(userId);

            // Assert
            Assert.True(result.Success, "Result should be successful");
            Assert.False(string.IsNullOrWhiteSpace(result.Data?.Token), "TokenValue should not be null or empty");

            // Verificar expiración
            var expectedExpiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Refresh));
            var actualExpiration = result.Data!.Expiration;
            var diffSeconds = Math.Abs((actualExpiration - expectedExpiration).TotalSeconds);
            Assert.True(diffSeconds < 5, "Expiration should match TokenPolicies");

            // Verificar que se persiste en repo
            _tokenRepositoryMock.Verify(r => r.AddAsync(It.Is<Token>(t => t.UserId == userId
                && t.Type == TokenType.Refresh
                && !string.IsNullOrEmpty(t.ExpiresAt.ToString()))), Times.Once);

            // Opcional: verificar que el hash no sea igual al token plano
            Assert.NotEqual(result.Data!.Token, _tokenRepositoryMock.Invocations[0].Arguments[0] is Token t ? t.ToString() : "");
        }
    }
}
