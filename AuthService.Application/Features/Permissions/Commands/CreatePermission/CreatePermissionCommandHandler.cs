using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.CreatePermission
{
    public class CreatePermissionCommandHandler(IPermissionService permissionService) : IRequestHandler<CreatePermissionCommand, Result<PermissionResponse>>
    {
        public async Task<Result<PermissionResponse>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            return await permissionService.CreateAsync(request.Name!, request.Description!);
        }
    }
}
