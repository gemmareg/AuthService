using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.UpdatePermissionDescription
{
    public class UpdatePermissionDescriptionCommandHandler(IPermissionService permissionService) : IRequestHandler<UpdatePermissionDescriptionCommand, Result>
    {
        public async Task<Result> Handle(UpdatePermissionDescriptionCommand request, CancellationToken cancellationToken)
        {
            return await permissionService.UpdateDescriptionAsync(request.PermissionId, request.Description!);
        }
    }
}
