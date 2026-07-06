using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, Result<RoleResponse>>
    {
        public async Task<Result<RoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.CreateAsync(request.Name!);
        }
    }
}
