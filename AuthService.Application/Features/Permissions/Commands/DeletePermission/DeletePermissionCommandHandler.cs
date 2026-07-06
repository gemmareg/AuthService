using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.DeletePermission
{
    public class DeletePermissionCommandHandler(IPermissionService permissionService) : IRequestHandler<DeletePermissionCommand, Result>
    {
        public async Task<Result> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            return await permissionService.DeleteAsync(request.PermissionId);
        }
    }
}
