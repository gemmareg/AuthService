using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest<Result>
    {
        public Guid RoleId { get; set; }
    }
}
