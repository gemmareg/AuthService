using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleCommandHandler(IRoleService roleService) : IRequestHandler<RemovePermissionFromRoleCommand, Result>
    {
        public async Task<Result> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.RemovePermissionAsync(request.RoleId, request.PermissionId);
        }
    }
}
