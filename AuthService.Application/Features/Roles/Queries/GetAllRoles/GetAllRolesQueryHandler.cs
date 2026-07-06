using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler(IRoleService roleService) : IRequestHandler<GetAllRolesQuery, Result<List<RoleResponse>>>
    {
        public async Task<Result<List<RoleResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            return await roleService.GetAllAsync();
        }
    }
}
