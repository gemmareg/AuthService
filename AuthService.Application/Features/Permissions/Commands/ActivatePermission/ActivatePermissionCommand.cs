using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.ActivatePermission
{
    public class ActivatePermissionCommand : IRequest<Result>
    {
        public Guid PermissionId { get; set; }
    }
}
