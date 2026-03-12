using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.SoftDeleteUser
{
    public class SoftDeleteUserCommandHandler(IUserService userService) : IRequestHandler<SoftDeleteUserCommand, Result>
    {
        public async Task<Result> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.SoftDeleteAsync(request.UserId, request.RequesterId);
        }
    }
}
