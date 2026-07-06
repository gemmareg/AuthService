using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Permissions.Queries.GetAllPermissions
{
    public class GetAllPermissionsQueryHandler(IPermissionService permissionService) : IRequestHandler<GetAllPermissionsQuery, Result<List<PermissionResponse>>>
    {
        public async Task<Result<List<PermissionResponse>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            return await permissionService.GetAllAsync();
        }
    }
}
