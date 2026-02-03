using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain;
using AuthService.Infrastructure.Persistance.Context;
using AuthService.Infrastructure.Persistance.Repositories.Common;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AuthDbContext context) : base(context)
        {
        }
    }
}
