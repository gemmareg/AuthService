using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;

namespace AuthService.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<Result<AuthResponse>> RegisterAsync(string name, string surname, string email, string password);
    }
}
