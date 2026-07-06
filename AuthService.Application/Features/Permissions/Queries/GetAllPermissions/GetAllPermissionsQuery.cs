using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Permissions.Queries.GetAllPermissions
{
    public class GetAllPermissionsQuery : IRequest<Result<List<PermissionResponse>>>
    {
    }
}
