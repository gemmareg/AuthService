using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler(IUserService userService) : IRequestHandler<CreateUserCommand, Result<AuthResponse>>
    {

        public async Task<Result<AuthResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.RegisterAsync(request.Name, request.Surname, request.Email, request.Password);
        }
    }
}
