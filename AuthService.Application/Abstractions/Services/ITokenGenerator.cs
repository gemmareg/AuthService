using AuthService.Application.Dtos;
using AuthService.Domain;
using AuthService.Shared.Result.Generic;

namespace AuthService.Application.Abstractions.Services
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(User user);
        Task<Result<RefreshToken>> GenerateRefreshToken(Guid userId);
    }
}
