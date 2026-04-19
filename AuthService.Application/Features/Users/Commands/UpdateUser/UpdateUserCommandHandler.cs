using AuthService.Application.Abstractions.Services;
using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler(IUserService userService) : IRequestHandler<UpdateUserCommand, Result>
    {
        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.UpdateAsync(new Guid(request.Id), request.Name, request.Surname, request.Email, new Guid(request.RequesterId));
        }
    }
}
