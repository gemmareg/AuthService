using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdQuery, Result<RoleResponse>>
    {
        public async Task<Result<RoleResponse>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            return await roleService.GetByIdAsync(request.RoleId);
        }
    }
}
