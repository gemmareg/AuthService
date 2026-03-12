using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<Result>
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
