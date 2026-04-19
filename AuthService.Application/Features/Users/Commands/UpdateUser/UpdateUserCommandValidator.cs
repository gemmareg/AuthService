using AuthService.Shared;
using FluentValidation;

namespace AuthService.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required.")
                .Must(ValidationHelpers.BeAValidGuid).WithMessage("Invalid User ID format.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MaximumLength(50).WithMessage("Surname cannot exceed 50 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.RequesterId)
                .NotEmpty().WithMessage("Requester ID is required.")
                .Must(ValidationHelpers.BeAValidGuid).WithMessage("Invalid Requester ID format.");
        }
    }
}
