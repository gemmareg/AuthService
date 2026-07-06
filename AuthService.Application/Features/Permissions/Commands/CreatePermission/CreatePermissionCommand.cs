using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.CreatePermission
{
    public class CreatePermissionCommand : IRequest<Result<PermissionResponse>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
