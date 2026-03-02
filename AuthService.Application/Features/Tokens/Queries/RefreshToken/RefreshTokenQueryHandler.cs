using AuthService.Application.Abstractions.Services;
using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Tokens.Queries.RefreshToken
{
    public class RefreshTokenQueryHandler(ITokenRefresher tokenRefresher) : IRequestHandler<RefreshTokenQuery, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            return await tokenRefresher.RefreshTokenAsync(request.RefreshToken);
        }
    }
}
