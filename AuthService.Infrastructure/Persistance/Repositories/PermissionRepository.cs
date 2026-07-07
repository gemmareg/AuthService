using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain;
using AuthService.Infrastructure.Persistance.Context;
using AuthService.Infrastructure.Persistance.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AuthDbContext dbContext) : base(dbContext)
        {
        }

        public Task<Permission?> GetByNameAsync(string name)
            => _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);
    }
}
