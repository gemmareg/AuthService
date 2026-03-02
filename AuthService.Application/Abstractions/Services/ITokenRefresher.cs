using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;

namespace AuthService.Application.Abstractions.Services
{
    public interface ITokenRefresher
    {
        Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken);
    }
}
