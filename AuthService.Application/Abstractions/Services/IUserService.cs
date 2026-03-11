using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;

namespace AuthService.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<Result<AuthResponse>> RegisterAsync(string name, string surname, string email, string password);
        Task<Result<AuthResponse>> LoginAsync(string email, string password);
        Task<Result> SoftDeleteAsync(Guid userId);
    }
}
