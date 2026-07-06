using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Application.Extensions.Options;
using AuthService.Application.Services;
using AuthService.Domain;
using AuthService.Domain.Policies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static AuthService.Shared.Enums;

namespace AuthService.Application.UnitTest.Services
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<TokenService>> _loggerMock = new();

        private JwtSettings JwtSettingsFixture => new()
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            SecretKey = "ThisIsASecretKeyForTestingPurposesOnly"
        };

        public TokenServiceTests()
        {
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<TokenService>>();
            var options = Options.Create(JwtSettingsFixture);
            _tokenService = new TokenService(_tokenRepositoryMock.Object, _permissionRepositoryMock.Object, options, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_ShouldContainClaimsAndCorrectExpiration()
        {
            // Arrange
            var email = "test@test.com";
            var user = User.Create("john", email, "hash", "John", "Doe", Guid.NewGuid()).Data!;
            var managerRole = Role.Create("manager").Data!;
            var staffRole = Role.Create("staff").Data!;
            var readPermission = Permission.Create("users.read", "Read users").Data!;
            var writePermission = Permission.Create("users.write", "Write users").Data!;

            managerRole.AddPermission(readPermission);
            staffRole.AddPermission(writePermission);
            user.AssignRole(managerRole);
            user.AssignRole(staffRole);
            user.AddPermissions(readPermission); // duplicate on purpose

            // Act
            var tokenString = await _tokenService.GenerateAccessTokenAsync(user);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(tokenString));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenString);

            Assert.Equal(user.Id.ToString(), jwt.Subject);
            Assert.Equal(email, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            foreach (var role in user.Roles.Select(r => r.Name))
            {
                Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == role);
            }
            foreach (var permission in new[] { "users.read", "users.write" })
            {
                Assert.Contains(jwt.Claims, c => c.Type == "permission" && c.Value == permission);
            }
            Assert.Equal(2, jwt.Claims.Count(c => c.Type == "permission"));

            var expectedExpiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Access));
            var difference = (jwt.ValidTo - expectedExpiration).TotalSeconds;
            Assert.True(Math.Abs(difference) < 5, "AccessToken expiration is not correct");

            _permissionRepositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GenerateAccessTokenAsync_Should_Include_All_Active_Permissions_For_Admin_Regardless_Of_Assignment()
        {
            // Arrange: Admin user with no permissions directly or via role assigned in-memory.
            var user = User.Create("admin-user", "admin@test.com", "hash", "Admin", "User", Guid.NewGuid()).Data!;
            var adminRole = Role.Create("Admin").Data!;
            user.AssignRole(adminRole);

            var activeA = Permission.Create("roles:read", "Read roles").Data!;
            var activeB = Permission.Create("permissions:read", "Read permissions").Data!;
            var inactive = Permission.Create("roles:delete", "Delete roles").Data!;
            inactive.Deactivate();

            _permissionRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Permission> { activeA, activeB, inactive });

            // Act
            var tokenString = await _tokenService.GenerateAccessTokenAsync(user);

            // Assert
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
            var permissionClaims = jwt.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

            Assert.Contains("roles:read", permissionClaims);
            Assert.Contains("permissions:read", permissionClaims);
            Assert.DoesNotContain("roles:delete", permissionClaims);
            Assert.Equal(2, permissionClaims.Count);

            _permissionRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GenerateRefreshToken_ShouldContainClaimsAndCorrectExpiration()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _tokenRepositoryMock.Reset();

            _tokenRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Token>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _tokenService.GenerateRefreshToken(userId);

            // Assert
            Assert.True(result.Success, "Result should be successful");
            Assert.False(string.IsNullOrWhiteSpace(result.Data?.Token), "TokenValue should not be null or empty");

            var expectedExpiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Refresh));
            var actualExpiration = result.Data!.Expiration;
            var diffSeconds = Math.Abs((actualExpiration - expectedExpiration).TotalSeconds);
            Assert.True(diffSeconds < 5, "Expiration should match TokenPolicies");

            _tokenRepositoryMock.Verify(r => r.AddAsync(It.Is<Token>(t => t.UserId == userId
                && t.Type == TokenType.Refresh
                && !string.IsNullOrEmpty(t.ExpiresAt.ToString()))), Times.Once);

            Assert.NotEqual(result.Data!.Token, _tokenRepositoryMock.Invocations[0].Arguments[0] is Token t ? t.ToString() : "");
        }
    }
}
