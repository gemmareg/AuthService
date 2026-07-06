using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.RenameRole
{
    public class RenameRoleCommand : IRequest<Result>
    {
        public Guid RoleId { get; set; }
        public string? Name { get; set; }
    }
}
