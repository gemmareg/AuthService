using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.UpdatePermissionDescription
{
    public class UpdatePermissionDescriptionCommand : IRequest<Result>
    {
        public Guid PermissionId { get; set; }
        public string? Description { get; set; }
    }
}
