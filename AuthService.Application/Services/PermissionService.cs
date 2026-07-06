using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.UnitOfWork;
using AuthService.Application.Dtos;
using AuthService.Domain;
using AuthService.Shared.Result.Generic;
using AuthService.Shared.Result.NonGeneric;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Services
{
    public class PermissionService(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<PermissionService> logger) : IPermissionService
    {
        public async Task<Result<PermissionResponse>> CreateAsync(string name, string description)
        {
            var permissionResult = Permission.Create(name, description);
            if (!permissionResult.Success)
            {
                return Result<PermissionResponse>.Fail(permissionResult.Message);
            }

            var permission = permissionResult.Data!;
            await permissionRepository.AddAsync(permission);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Permission created successfully with ID: {PermissionId}", permission.Id);

            return Result<PermissionResponse>.Ok(ToResponse(permission));
        }

        public async Task<Result<PermissionResponse>> GetByIdAsync(Guid permissionId)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission is null)
            {
                return Result<PermissionResponse>.Fail("Permission not found");
            }

            return Result<PermissionResponse>.Ok(ToResponse(permission));
        }

        public async Task<Result<List<PermissionResponse>>> GetAllAsync()
        {
            var permissions = await permissionRepository.GetAllAsync();
            return Result<List<PermissionResponse>>.Ok(permissions.Select(ToResponse).ToList());
        }

        public async Task<Result> UpdateDescriptionAsync(Guid permissionId, string description)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission is null)
            {
                return Result.Fail("Permission not found");
            }

            permission.UpdateDescription(description);

            await permissionRepository.UpdateAsync(permission);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Permission description updated. PermissionId: {PermissionId}", permissionId);

            return Result.Ok();
        }

        public async Task<Result> ActivateAsync(Guid permissionId)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission is null)
            {
                return Result.Fail("Permission not found");
            }

            permission.Activate();

            await permissionRepository.UpdateAsync(permission);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Permission activated. PermissionId: {PermissionId}", permissionId);

            return Result.Ok();
        }

        public async Task<Result> DeactivateAsync(Guid permissionId)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission is null)
            {
                return Result.Fail("Permission not found");
            }

            permission.Deactivate();

            await permissionRepository.UpdateAsync(permission);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Permission deactivated. PermissionId: {PermissionId}", permissionId);

            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(Guid permissionId)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission is null)
            {
                return Result.Fail("Permission not found");
            }

            await permissionRepository.RemoveAsync(permission);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Permission deleted. PermissionId: {PermissionId}", permissionId);

            return Result.Ok();
        }

        private static PermissionResponse ToResponse(Permission permission) => new()
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            IsActive = permission.IsActive
        };
    }
}
