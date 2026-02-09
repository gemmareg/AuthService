using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Application.Services;
using AuthService.Domain;
using AuthService.Domain.Policies;
using AuthService.Shared;
using AuthService.Shared.Result.Generic;
using Moq;
using static AuthService.Shared.Enums;

namespace AuthService.Application.UnitTest.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IUserService _userService;

        private const string USERNAME = "johndoe";
        private const string EMAIL = "johndoe@email.com";
        private const string PASSWORD = "1234";
        private const string PASSWORD_HASHED = "100000.rN/ubQNjmQc6gOGSNb/YpA==.f38ttjvad+XnI37+lQ9BIG2q97B7xIdeoxkg4emK39Q=";
        private const string NAME = "John";
        private const string SURNAME = "Doe";

        public UserServiceTests()
        {
            _passwordServiceMock = new Mock<IPasswordService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(
                _passwordServiceMock.Object,
                _tokenServiceMock.Object,
                _userRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFail_EmailAlreadyRegistered()
        {
            // Arrange
            var user = User.Create(USERNAME, EMAIL, PASSWORD, NAME, SURNAME).Data!;
            _userRepositoryMock.Setup(p => p.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            var result = await _userService.RegisterAsync(NAME, SURNAME, EMAIL, PASSWORD);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email already registered", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFail_EmptyEmail()
        {
            // Arrange
            _passwordServiceMock.Setup(p => p.Hash(It.IsAny<string>())).Returns(PASSWORD_HASHED);
            _userRepositoryMock.Setup(p => p.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync([]);

            // Act
            var result = await _userService.RegisterAsync(NAME, SURNAME, string.Empty, PASSWORD);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.EMAIL_NOT_NULL, result.Message);
        }

        [Theory]
        [InlineData(USERNAME, USERNAME + "1")]
        [InlineData(USERNAME+"1", USERNAME + "2")]
        [InlineData("abc"+USERNAME+"abc", USERNAME)]
        public async Task RegisterAsync_ReturnsOk_WithNormalUserName(string username, string expectedUsername)
        {
            // Arrange
            var existingusers = new List<User>()
            {
                User.Create(username, EMAIL, PASSWORD, NAME, SURNAME).Data!
            };
            var refreshToken = new RefreshToken()
            {
                Expiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Refresh)),
                Token = "AqpQs7zNU+C6CLfp/TO3CL/Bm1KxJ/vh99En/SnV1O75xSQm7M4exLw0Crwi9If/4WjsiAgbjl534WhLFoIfBA=="
            };
            _passwordServiceMock.Setup(p => p.Hash(It.IsAny<string>())).Returns(PASSWORD_HASHED);
            _userRepositoryMock.Setup(p => p.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(existingusers);
            _tokenServiceMock.Setup(p => p.GenerateRefreshToken(It.IsAny<Guid>())).ReturnsAsync(Result<RefreshToken>.Ok(refreshToken));

            // Act
            var result = await _userService.RegisterAsync(NAME, SURNAME, EMAIL, PASSWORD);

            // Assert
            Assert.True(result.Success);
            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Username == expectedUsername)),Times.Once);
        }
    }
}
