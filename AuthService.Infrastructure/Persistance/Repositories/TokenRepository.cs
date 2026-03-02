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
        {
            return await _context.Tokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }
    }
}
