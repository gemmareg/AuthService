using AuthService.Application.Abstractions.Repositories.Common;
using AuthService.Domain;

namespace AuthService.Application.Abstractions.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByEmailWithRolesAsync(string email);

        Task<User?> GetByIdWithRolesAsync(Guid id);

        Task<List<User>> GetByUsernameAsync(string username);
    }
}
