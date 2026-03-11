using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.SoftDeleteUser
{
    public class SoftDeleteUserCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
    }
}
