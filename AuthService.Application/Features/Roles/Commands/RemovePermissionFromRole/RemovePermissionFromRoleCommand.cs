using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleCommand : IRequest<Result>
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
