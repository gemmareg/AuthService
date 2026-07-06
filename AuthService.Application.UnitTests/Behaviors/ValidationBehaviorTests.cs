using AuthService.Application.Behaviors;
using AuthService.Application.Dtos;
using AuthService.Application.Features.Users.Commands.CreateUser;
using AuthService.Application.Security;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;
using FluentValidation;

namespace AuthService.Application.UnitTest.Behaviors
{
    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Handle_Should_Short_Circuit_With_Result_Fail_When_Command_Is_Invalid()
        {
            var behavior = new ValidationBehavior<CreateUserCommand, Result<AuthResponse>>(
                new[] { new CreateUserCommandValidator(new CommonPasswordChecker()) });

            var invalidCommand = new CreateUserCommand
            {
                Name = "",
                Surname = "Doe",
                Email = "not-an-email",
                Password = "123"
            };

            var nextCalled = false;

            var result = await behavior.Handle(invalidCommand, _ =>
            {
                nextCalled = true;
                return Task.FromResult(Result<AuthResponse>.Ok(new AuthResponse()));
            }, CancellationToken.None);

            Assert.False(nextCalled);
            Assert.False(result.Success);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        }

        [Fact]
        public async Task Handle_Should_Call_Next_When_Command_Is_Valid()
        {
            var behavior = new ValidationBehavior<CreateUserCommand, Result<AuthResponse>>(
                new[] { new CreateUserCommandValidator(new CommonPasswordChecker()) });

            var validCommand = new CreateUserCommand
            {
                Name = "Jane",
                Surname = "Doe",
                Email = "jane@example.com",
                Password = "SomeValidPassword1"
            };

            var expected = Result<AuthResponse>.Ok(new AuthResponse());

            var result = await behavior.Handle(validCommand, _ => Task.FromResult(expected), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Same(expected, result);
        }

        [Fact]
        public async Task Handle_Should_Short_Circuit_With_NonGeneric_Result_Fail_When_Invalid()
        {
            var behavior = new ValidationBehavior<TestCommand, Result>(
                new[] { new TestCommandValidator() });

            var nextCalled = false;

            var result = await behavior.Handle(new TestCommand { Value = "" }, _ =>
            {
                nextCalled = true;
                return Task.FromResult(Result.Ok());
            }, CancellationToken.None);

            Assert.False(nextCalled);
            Assert.False(result.Success);
        }

        public class TestCommand : MediatR.IRequest<Result>
        {
            public string? Value { get; set; }
        }

        public class TestCommandValidator : AbstractValidator<TestCommand>
        {
            public TestCommandValidator()
            {
                RuleFor(x => x.Value).NotEmpty();
            }
        }
    }
}
