using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Application.Features.Users.Queries.GetUser;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler(IUserService userService) : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdQueryResponse>>
    {
        public async Task<Result<GetUserByIdQueryResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await userService.GetUserByIdAsync(new Guid(request.Id));
        }
    }
}
