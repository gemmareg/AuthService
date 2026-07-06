using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand, Result>
    {
        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.DeleteAsync(request.RoleId);
        }
    }
}
