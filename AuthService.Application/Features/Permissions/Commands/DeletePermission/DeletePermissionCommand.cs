using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.DeletePermission
{
    public class DeletePermissionCommand : IRequest<Result>
    {
        public Guid PermissionId { get; set; }
    }
}
