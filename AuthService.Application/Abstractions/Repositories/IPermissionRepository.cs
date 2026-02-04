using AuthService.Application.Abstractions.Repositories.Common;
using AuthService.Domain;

namespace AuthService.Application.Abstractions.Repositories
{
    public interface IPermissionRepository : IRepository<Permission>
    {
    }
}
