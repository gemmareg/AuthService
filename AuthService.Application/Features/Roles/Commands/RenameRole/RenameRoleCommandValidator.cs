using FluentValidation;

namespace AuthService.Application.Features.Roles.Commands.RenameRole
{
    public class RenameRoleCommandValidator : AbstractValidator<RenameRoleCommand>
    {
        public RenameRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        }
    }
}
