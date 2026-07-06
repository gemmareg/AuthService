using FluentValidation;

namespace AuthService.Application.Features.Permissions.Commands.DeactivatePermission
{
    public class DeactivatePermissionCommandValidator : AbstractValidator<DeactivatePermissionCommand>
    {
        public DeactivatePermissionCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("PermissionId is required");
        }
    }
}
