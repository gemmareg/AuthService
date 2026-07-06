using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.DeactivatePermission
{
    public class DeactivatePermissionCommand : IRequest<Result>
    {
        public Guid PermissionId { get; set; }
    }
}
