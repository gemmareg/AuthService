using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Users.Queries.GetUser
{
    public class GetUserByIdQuery : IRequest<Result<GetUserByIdQueryResponse>>
    {
        public string? Id { get; set; }
    }
}
