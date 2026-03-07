using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.LoginUser
{
    public class LoginUserCommandHandler(IUserService userService) : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.LoginAsync(request.Email, request.Password);
        }
    }
}
