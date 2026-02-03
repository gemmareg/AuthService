using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain;
using AuthService.Infrastructure.Persistance.Context;
using AuthService.Infrastructure.Persistance.Repositories.Common;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(AuthDbContext context) : base(context)
        {
        }
    }
}
