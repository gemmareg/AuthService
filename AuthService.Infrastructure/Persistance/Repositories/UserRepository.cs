using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain;
using AuthService.Infrastructure.Persistance.Context;
using AuthService.Infrastructure.Persistance.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AuthDbContext context) : base(context)
        {
        }

        public Task<User?> GetByEmailAsync(string email)
            => _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public Task<User?> GetByEmailWithRolesAsync(string email)
            => _context.Users
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Permissions)
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Email == email);


        public Task<User?> GetByIdWithRolesAsync(Guid id)
            => _context.Users
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Permissions)
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Id == id);

        public Task<List<User>> GetByUsernameAsync(string username)
            => _context.Users.Where(u => u.Username.StartsWith(username)).ToListAsync();
    }
}
