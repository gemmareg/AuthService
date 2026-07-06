using FluentValidation;

namespace AuthService.Application.Features.Permissions.Commands.DeletePermission
{
    public class DeletePermissionCommandValidator : AbstractValidator<DeletePermissionCommand>
    {
        public DeletePermissionCommandValidator()
        {
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("PermissionId is required");
        }
    }
}
