using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommandHandler(IRoleService roleService) : IRequestHandler<AssignPermissionToRoleCommand, Result>
    {
        public async Task<Result> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.AssignPermissionAsync(request.RoleId, request.PermissionId);
        }
    }
}
