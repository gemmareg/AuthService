using FluentValidation;

namespace AuthService.Application.Features.Permissions.Commands.UpdatePermissionDescription
{
    public class UpdatePermissionDescriptionCommandValidator : AbstractValidator<UpdatePermissionDescriptionCommand>
    {
        public UpdatePermissionDescriptionCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("PermissionId is required");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
