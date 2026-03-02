using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Tokens.Queries.RefreshToken
{
    public class RefreshTokenQuery : IRequest<Result<AuthResponse>>
    {
        public string RefreshToken { get; set; }
    }
}
