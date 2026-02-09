using AuthService.Application.Abstractions.Repositories.Common;
using AuthService.Domain;

namespace AuthService.Application.Abstractions.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<List<User>> GetByUsernameAsync(string username);
    }
}
