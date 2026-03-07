using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<Result<AuthResponse>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
