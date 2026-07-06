using FluentValidation;

namespace AuthService.Application.Features.Permissions.Commands.CreatePermission
{
    public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
