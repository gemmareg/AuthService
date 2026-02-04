using AuthService.Application.Abstractions.Repositories;
using AuthService.Infrastructure.IntegrationTests.Fixtures;
using AuthService.Infrastructure.Persistance.Repositories;

namespace AuthService.Infrastructure.IntegrationTests.Repositories
{
    [Collection("Database collection")]
    public class RoleRepositoryTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public RoleRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _roleRepository = new RoleRepository(_databaseFixture.DbContext);
            _permissionRepository = new PermissionRepository(_databaseFixture.DbContext);
        }

        [Fact]
        public async Task PersistRole_BasicCRD()
        {
            // Arrange
            var role = Domain.Role.Create("Admin").Data!;

            // Create role
            await _roleRepository.AddAsync(role);
            await _databaseFixture.DbContext.SaveChangesAsync();
            
            // Read role
            var retrievedRole = _roleRepository.GetByIdAsync(role.Id).Result;
            Assert.NotNull(retrievedRole);
                        
            // Delete role
            _roleRepository.RemoveAsync(retrievedRole).Wait();
            _databaseFixture.DbContext.SaveChangesAsync().Wait();
            var deletedRole = _roleRepository.GetByIdAsync(role.Id).Result;
            Assert.Null(deletedRole);
        }

        [Fact]
        public async Task PersistRole_ManagePermissions()
        {
            // Arrange
            var role = Domain.Role.Create("Admin").Data!;
            var permission = Domain.Permission.Create("User Management", "read:users").Data!;

            // Create role and permission
            await _roleRepository.AddAsync(role);
            await _permissionRepository.AddAsync(permission);
            await _databaseFixture.DbContext.SaveChangesAsync();

            // Assign permission to role
            role.AddPermission(permission);
            await _databaseFixture.DbContext.SaveChangesAsync();

            // Read role and verify permission assignment
            var retrievedRole = await _roleRepository.GetByIdAsync(role.Id);
            Assert.NotNull(retrievedRole);
            Assert.Contains(retrievedRole!.Permissions, p => p.Id == permission.Id);
        }
    }
}
