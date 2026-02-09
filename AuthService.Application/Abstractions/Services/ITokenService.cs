using AuthService.Application.Dtos;
using AuthService.Domain;
using AuthService.Shared.Result.Generic;

namespace AuthService.Application.Abstractions.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
        Task<Result<RefreshToken>> GenerateRefreshToken(Guid userId);
    }
}
