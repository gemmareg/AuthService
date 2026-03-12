using FluentValidation;

namespace AuthService.Application.Features.Users.Commands.SoftDeleteUser
{
    public class SoftDeleteUserCommandValidator : AbstractValidator<SoftDeleteUserCommand>
    {
        public SoftDeleteUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.RequesterId)
                .NotEmpty().WithMessage("RequesterId is required");
        }
    }
}
