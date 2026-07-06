using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.DeactivatePermission
{
    public class DeactivatePermissionCommandHandler(IPermissionService permissionService) : IRequestHandler<DeactivatePermissionCommand, Result>
    {
        public async Task<Result> Handle(DeactivatePermissionCommand request, CancellationToken cancellationToken)
        {
            return await permissionService.DeactivateAsync(request.PermissionId);
        }
    }
}
