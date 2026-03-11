using AuthService.Shared;

namespace AuthService.Domain.UnitTest
{
    public class UserTests
    {
        [Fact]
        public void Create_Should_Create_User_When_Data_Is_Valid()
        {
            // Act
            var result = User.Create("johndoe", "johndoe@email.com","1234","John","Doe");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("johndoe", result.Data!.Username);
            Assert.Equal("johndoe@email.com", result.Data!.Email);
            Assert.True(result.Data!.IsActive);
        }

        [Fact]
        public void Create_Should_Fail_When_Username_Is_Empty()
        {
            var result = User.Create("", "test@email.com","1234", null, null);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.USERNAME_NOT_NULL, result.Message);
        }

        [Fact]
        public void Create_Should_Fail_When_Email_Is_Empty()
        {
            var result = User.Create("user", "", "1234", null, null);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.EMAIL_NOT_NULL, result.Message);
        }
        [Fact]
        public void SoftDelete_Should_Set_IsActive_To_False_When_User_Is_Active()
        {
            var user = User.Create("johndoe", "johndoe@email.com", "1234", "John", "Doe").Data!;

            var result = user.SoftDelete();

            Assert.True(result.Success);
            Assert.False(user.IsActive);
        }

        [Fact]
        public void SoftDelete_Should_Fail_When_User_Is_Already_Inactive()
        {
            var user = User.Create("johndoe", "johndoe@email.com", "1234", "John", "Doe").Data!;
            user.SoftDelete();

            var result = user.SoftDelete();

            Assert.False(result.Success);
            Assert.Equal("User is already inactive", result.Message);
        }

    }
}
