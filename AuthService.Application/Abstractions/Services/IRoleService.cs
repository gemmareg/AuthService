using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;

namespace AuthService.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<Result<RoleResponse>> CreateAsync(string name);
        Task<Result<RoleResponse>> GetByIdAsync(Guid roleId);
        Task<Result<List<RoleResponse>>> GetAllAsync();
        Task<Result> RenameAsync(Guid roleId, string name);
        Task<Result> DeleteAsync(Guid roleId);
        Task<Result> AssignPermissionAsync(Guid roleId, Guid permissionId);
        Task<Result> RemovePermissionAsync(Guid roleId, Guid permissionId);
    }
}
