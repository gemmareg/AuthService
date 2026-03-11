using AuthService.Application.Abstractions.Events;
using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Application.Dtos;
using AuthService.Application.Services;
using AuthService.Domain;
using AuthService.Domain.Policies;
using AuthService.Shared;
using AuthService.Shared.Result.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using static AuthService.Shared.Enums;

namespace AuthService.Application.UnitTest.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<ITokenGenerator> _tokenServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IUserService _userService;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IEventPublisher> _eventPublisher;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<UserService>> _loggerMock = new();


        private const string USERNAME = "johndoe";
        private const string EMAIL = "johndoe@email.com";
        private const string PASSWORD = "1234";
        private const string PASSWORD_HASHED = "100000.rN/ubQNjmQc6gOGSNb/YpA==.f38ttjvad+XnI37+lQ9BIG2q97B7xIdeoxkg4emK39Q=";
        private const string NAME = "John";
        private const string SURNAME = "Doe";

        public UserServiceTests()
        {
            _passwordServiceMock = new Mock<IPasswordService>();
            _tokenServiceMock = new Mock<ITokenGenerator>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _eventPublisher = new Mock<IEventPublisher>();
            _unitOfWorkMock = new Mock<IUnitOfWork>(); 
            _loggerMock = new Mock<ILogger<UserService>>();
            _userService = new UserService(
                _passwordServiceMock.Object,
                _tokenServiceMock.Object,
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _eventPublisher.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);

            _roleRepositoryMock.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(Role.Create("User").Data!);
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


        [Fact]
        public async Task LoginAsync_ReturnsFail_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(p => p.GetByEmailWithRolesAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.LoginAsync(EMAIL, PASSWORD);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFail_WhenPasswordIsInvalid()
        {
            // Arrange
            var user = User.Create(USERNAME, EMAIL, PASSWORD_HASHED, NAME, SURNAME).Data!;
            _userRepositoryMock.Setup(p => p.GetByEmailWithRolesAsync(It.IsAny<string>())).ReturnsAsync(user);
            _passwordServiceMock.Setup(p => p.Verify(PASSWORD, PASSWORD_HASHED)).Returns(false);

            // Act
            var result = await _userService.LoginAsync(EMAIL, PASSWORD);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var user = User.Create(USERNAME, EMAIL, PASSWORD_HASHED, NAME, SURNAME).Data!;
            user.AssignRole(Role.Create("User").Data!);

            var refreshToken = new RefreshToken
            {
                Expiration = DateTime.UtcNow.Add(TokenPolicies.GetExpiration(TokenType.Refresh)),
                Token = "valid-refresh-token"
            };

            _userRepositoryMock.Setup(p => p.GetByEmailWithRolesAsync(It.IsAny<string>())).ReturnsAsync(user);
            _passwordServiceMock.Setup(p => p.Verify(PASSWORD, PASSWORD_HASHED)).Returns(true);
            _tokenServiceMock.Setup(p => p.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns("valid-access-token");
            _tokenServiceMock.Setup(p => p.GenerateRefreshToken(It.IsAny<Guid>())).ReturnsAsync(Result<RefreshToken>.Ok(refreshToken));

            // Act
            var result = await _userService.LoginAsync(EMAIL, PASSWORD);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("valid-access-token", result.Data!.AccessToken);
            Assert.Equal("valid-refresh-token", result.Data.RefreshToken);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsFail_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.SoftDeleteAsync(Guid.NewGuid());

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not found", result.Message);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var user = User.Create(USERNAME, EMAIL, PASSWORD_HASHED, NAME, SURNAME).Data!;
            _userRepositoryMock.Setup(p => p.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(user);

            // Act
            var result = await _userService.SoftDeleteAsync(user.Id);

            // Assert
            Assert.True(result.Success);
            Assert.False(user.IsActive);
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == user.Id && !u.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _eventPublisher.Verify(e => e.PublishUserSoftDeleted(It.IsAny<Auth.Contracts.Events.UserSoftDeletedEvent>()), Times.Once);
        }

    }
}
