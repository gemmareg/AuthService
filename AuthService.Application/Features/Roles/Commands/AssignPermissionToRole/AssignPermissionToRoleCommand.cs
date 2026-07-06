using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommand : IRequest<Result>
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
