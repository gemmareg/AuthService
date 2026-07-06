using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.RenameRole
{
    public class RenameRoleCommandHandler(IRoleService roleService) : IRequestHandler<RenameRoleCommand, Result>
    {
        public async Task<Result> Handle(RenameRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.RenameAsync(request.RoleId, request.Name!);
        }
    }
}
