using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain;
using AuthService.Infrastructure.Persistance.Context;
using AuthService.Infrastructure.Persistance.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class TokenRepository : Repository<Token>, ITokenRepository
    {
        public TokenRepository(AuthDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Token?> GetByTokenHashAsync(string tokenHash)
            => await _context.Tokens
                .Include(t => t.User)
                    .ThenInclude(u => u.Roles)
                        .ThenInclude(r => r.Permissions)
                .Include(t => t.User)
                    .ThenInclude(u => u.Permissions)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
    }
}
