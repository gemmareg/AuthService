using FluentValidation;

namespace AuthService.Application.Features.Roles.Commands.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
    {
        public AssignPermissionToRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required");
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("PermissionId is required");
        }
    }
}
