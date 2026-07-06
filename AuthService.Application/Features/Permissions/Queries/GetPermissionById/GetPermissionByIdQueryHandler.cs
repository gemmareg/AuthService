using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Permissions.Queries.GetPermissionById
{
    public class GetPermissionByIdQueryHandler(IPermissionService permissionService) : IRequestHandler<GetPermissionByIdQuery, Result<PermissionResponse>>
    {
        public async Task<Result<PermissionResponse>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            return await permissionService.GetByIdAsync(request.PermissionId);
        }
    }
}
