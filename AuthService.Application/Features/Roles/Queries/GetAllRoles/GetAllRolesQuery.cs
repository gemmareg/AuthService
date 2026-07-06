using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<Result<List<RoleResponse>>>
    {
    }
}
