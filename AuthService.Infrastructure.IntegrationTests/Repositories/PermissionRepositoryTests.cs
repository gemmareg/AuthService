using AuthService.Application.Abstractions.Repositories;
using AuthService.Infrastructure.IntegrationTests.Fixtures;
using AuthService.Infrastructure.Persistance.Repositories;

namespace AuthService.Infrastructure.IntegrationTests.Repositories
{
    [Collection("Database collection")]
    public class PermissionRepositoryTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IPermissionRepository _permissionRepository;

        public PermissionRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _permissionRepository = new PermissionRepository(databaseFixture.DbContext);
        }

        [Fact]
        public async Task PersistPermission_BasicCRID()
        {
            // Arrange
            var permission = Domain.Permission.Create("User Management", "read:users").Data!;
            
            // Create permission
            await _permissionRepository.AddAsync(permission);
            await _databaseFixture.DbContext.SaveChangesAsync();
            
            // Read permission
            var retrievedPermission = await _permissionRepository.GetByIdAsync(permission.Id);
            Assert.NotNull(retrievedPermission);

            // Update permission
            retrievedPermission!.UpdateDescription("read:all_users");
            retrievedPermission!.Deactivate();
            await _permissionRepository.UpdateAsync(retrievedPermission);
            await _databaseFixture.DbContext.SaveChangesAsync();

            // Verify update
            var updatedPermission = await _permissionRepository.GetByIdAsync(permission.Id);
            Assert.NotNull(updatedPermission);
            Assert.Equal("read:all_users", updatedPermission!.Description);
            Assert.False(updatedPermission!.IsActive);

            // Delete permission
            await _permissionRepository.RemoveAsync(retrievedPermission);
            await _databaseFixture.DbContext.SaveChangesAsync();

            var deletedPermission = await _permissionRepository.GetByIdAsync(permission.Id);
            Assert.Null(deletedPermission);
        }
    }
}
