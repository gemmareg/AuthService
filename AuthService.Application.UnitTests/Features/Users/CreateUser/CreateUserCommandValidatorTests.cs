using AuthService.Application.Features.Users.Commands.CreateUser;
using AuthService.Application.Security;

namespace AuthService.Application.UnitTest.Features.Users.CreateUser
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator = new(new CommonPasswordChecker());

        [Fact]
        public void Validate_Should_Fail_When_Password_Is_Common()
        {
            var command = new CreateUserCommand
            {
                Name = "Jane",
                Surname = "Doe",
                Email = "jane@example.com",
                Password = "password1"
            };

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("too common"));
        }

        [Fact]
        public void Validate_Should_Succeed_When_Password_Is_Not_Common()
        {
            var command = new CreateUserCommand
            {
                Name = "Jane",
                Surname = "Doe",
                Email = "jane@example.com",
                Password = "Xk9#mQ2vT!pL7zR4"
            };

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }
    }
}
