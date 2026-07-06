using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.ActivatePermission
{
    public class ActivatePermissionCommandHandler(IPermissionService permissionService) : IRequestHandler<ActivatePermissionCommand, Result>
    {
        public async Task<Result> Handle(ActivatePermissionCommand request, CancellationToken cancellationToken)
        {
            return await permissionService.ActivateAsync(request.PermissionId);
        }
    }
}
