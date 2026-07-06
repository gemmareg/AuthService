using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;

namespace AuthService.Application.Abstractions.Services
{
    public interface IPermissionService
    {
        Task<Result<PermissionResponse>> CreateAsync(string name, string description);
        Task<Result<PermissionResponse>> GetByIdAsync(Guid permissionId);
        Task<Result<List<PermissionResponse>>> GetAllAsync();
        Task<Result> UpdateDescriptionAsync(Guid permissionId, string description);
        Task<Result> ActivateAsync(Guid permissionId);
        Task<Result> DeactivateAsync(Guid permissionId);
        Task<Result> DeleteAsync(Guid permissionId);
    }
}
