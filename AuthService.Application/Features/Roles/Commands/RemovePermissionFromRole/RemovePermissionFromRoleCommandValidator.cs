using FluentValidation;

namespace AuthService.Application.Features.Roles.Commands.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleCommandValidator : AbstractValidator<RemovePermissionFromRoleCommand>
    {
        public RemovePermissionFromRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required");
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("PermissionId is required");
        }
    }
}
