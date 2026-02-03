using AuthService.Shared;

namespace AuthService.Domain.UnitTest
{
    public class UserTests
    {
        [Fact]
        public void Create_Should_Create_User_When_Data_Is_Valid()
        {
            // Act
            var result = User.Create("jose", "jose@email.com");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("jose", result.Data!.Username);
            Assert.Equal("jose@email.com", result.Data!.Email);
            Assert.True(result.Data!.IsActive);
        }

        [Fact]
        public void Create_Should_Fail_When_Username_Is_Empty()
        {
            var result = User.Create("", "test@email.com");

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.USERNAME_NOT_NULL, result.Message);
        }

        [Fact]
        public void Create_Should_Fail_When_Email_Is_Empty()
        {
            var result = User.Create("user", "");

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.EMAIL_NOT_NULL, result.Message);
        }
    }
}
