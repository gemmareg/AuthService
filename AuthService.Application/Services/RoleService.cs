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
    public class RoleService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<RoleService> logger) : IRoleService
    {
        public async Task<Result<RoleResponse>> CreateAsync(string name)
        {
            var existingRole = await roleRepository.GetByNameAsync(name);
            if (existingRole is not null)
            {
                logger.LogWarning("Role creation attempted with already existing name: {Name}", name);
                return Result<RoleResponse>.Fail("A role with this name already exists");
            }

            var roleResult = Role.Create(name);
            if (!roleResult.Success)
            {
                return Result<RoleResponse>.Fail(roleResult.Message);
            }

            var role = roleResult.Data!;
            await roleRepository.AddAsync(role);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Role created successfully with ID: {RoleId}", role.Id);

            return Result<RoleResponse>.Ok(ToResponse(role));
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(Guid roleId)
        {
            var role = await roleRepository.GetByIdWithPermissionsAsync(roleId);
            if (role is null)
            {
                return Result<RoleResponse>.Fail("Role not found");
            }

            return Result<RoleResponse>.Ok(ToResponse(role));
        }

        public async Task<Result<List<RoleResponse>>> GetAllAsync()
        {
            var roles = await roleRepository.GetAllAsync();
            return Result<List<RoleResponse>>.Ok(roles.Select(ToResponse).ToList());
        }

        public async Task<Result> RenameAsync(Guid roleId, string name)
        {
            var role = await roleRepository.GetByIdAsync(roleId);
            if (role is null)
            {
                return Result.Fail("Role not found");
            }

            var existingRole = await roleRepository.GetByNameAsync(name);
            if (existingRole is not null && existingRole.Id != roleId)
            {
                return Result.Fail("A role with this name already exists");
            }

            var renameResult = role.Rename(name);
            if (!renameResult.Success)
            {
                return Result.Fail(renameResult.Message);
            }

            await roleRepository.UpdateAsync(role);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Role renamed successfully. RoleId: {RoleId}", roleId);

            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(Guid roleId)
        {
            var role = await roleRepository.GetByIdAsync(roleId);
            if (role is null)
            {
                return Result.Fail("Role not found");
            }

            await roleRepository.RemoveAsync(role);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Role deleted successfully. RoleId: {RoleId}", roleId);

            return Result.Ok();
        }

        public async Task<Result> AssignPermissionAsync(Guid roleId, Guid permissionId)
        {
            var role = await roleRepository.GetByIdWithPermissionsAsync(roleId);
            if (role is null)
            {
                return Result.Fail("Role not found");
            }

            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission is null)
            {
                return Result.Fail("Permission not found");
            }

            var addResult = role.AddPermission(permission);
            if (!addResult.Success)
            {
                return Result.Fail(addResult.Message);
            }

            await roleRepository.UpdateAsync(role);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Permission {PermissionId} assigned to role {RoleId}", permissionId, roleId);

            return Result.Ok();
        }

        public async Task<Result> RemovePermissionAsync(Guid roleId, Guid permissionId)
        {
            var role = await roleRepository.GetByIdWithPermissionsAsync(roleId);
            if (role is null)
            {
                return Result.Fail("Role not found");
            }

            var permission = role.Permissions.FirstOrDefault(p => p.Id == permissionId);
            if (permission is null)
            {
                return Result.Fail("Permission is not assigned to this role.");
            }

            var removeResult = role.RemovePermission(permission);
            if (!removeResult.Success)
            {
                return Result.Fail(removeResult.Message);
            }

            await roleRepository.UpdateAsync(role);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Permission {PermissionId} removed from role {RoleId}", permissionId, roleId);

            return Result.Ok();
        }

        private static RoleResponse ToResponse(Role role) => new()
        {
            Id = role.Id,
            Name = role.Name,
            Permissions = role.Permissions.Select(p => p.Name).ToList()
        };
    }
}
