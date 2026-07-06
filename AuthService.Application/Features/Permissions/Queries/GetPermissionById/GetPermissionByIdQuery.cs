using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Permissions.Queries.GetPermissionById
{
    public class GetPermissionByIdQuery : IRequest<Result<PermissionResponse>>
    {
        public Guid PermissionId { get; set; }
    }
}
