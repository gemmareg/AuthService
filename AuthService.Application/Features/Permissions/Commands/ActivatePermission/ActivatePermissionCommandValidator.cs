using FluentValidation;

namespace AuthService.Application.Features.Permissions.Commands.ActivatePermission
{
    public class ActivatePermissionCommandValidator : AbstractValidator<ActivatePermissionCommand>
    {
        public ActivatePermissionCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("PermissionId is required");
        }
    }
}
