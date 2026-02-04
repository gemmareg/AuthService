using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain;
using AuthService.Infrastructure.IntegrationTests.Fixtures;
using AuthService.Infrastructure.Persistance.Repositories;

namespace AuthService.Infrastructure.IntegrationTests.Repositories
{
    [Collection("Database collection")]
    public class UserRepositoryTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;

        public UserRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _userRepository = new UserRepository(_databaseFixture.DbContext);
            _roleRepository = new RoleRepository(_databaseFixture.DbContext);
            _permissionRepository = new PermissionRepository(_databaseFixture.DbContext);
        }

        [Fact]
        public async Task PersistUser_BasicCRUD()
        {
            // Arrange
            var user = User.Create("johndoe", "johndoe@test.com", "hashedpassword", "John", "Doe").Data!;

            // Create user
            await _userRepository.AddAsync(user);
            await _databaseFixture.DbContext.SaveChangesAsync();

            // Read user
            var retrievedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.NotNull(retrievedUser);

            // Update user: data, roles and permissions

            //update data
            retrievedUser.UpdateData("Johnny", "Doer", "johnnydoer@test.com");
            retrievedUser.UpdatePassword("newhashedpassword");
            await _databaseFixture.DbContext.SaveChangesAsync();

            var updatedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.Equal("Johnny", updatedUser!.Name);
            Assert.Equal("Doer", updatedUser.Surname);
            Assert.Equal("johnnydoer@test.com", updatedUser.Email);
            Assert.Equal("newhashedpassword", updatedUser.PasswordHash);

            // Delete user
            await _userRepository.RemoveAsync(retrievedUser);
            await _databaseFixture.DbContext.SaveChangesAsync();

            var deletedUser = await _userRepository.GetByIdAsync(user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task PersistUser_ManageRoles()
        {
            // Arrange
            var role = Role.Create("admin").Data!;
            var user = User.Create("johndoe", "johndoe@test.com", "hashedpassword", "John", "Doe").Data!;

            // Create user
            await _userRepository.AddAsync(user);
            await _roleRepository.AddAsync(role);
            await _databaseFixture.DbContext.SaveChangesAsync();

            // Get user
            var retrievedUser = await _userRepository.GetByIdAsync(user.Id);

            // assign role
            retrievedUser.AssignRole(role);
            await _databaseFixture.DbContext.SaveChangesAsync();

            var userWithRole = await _userRepository.GetByIdAsync(retrievedUser.Id);
            Assert.Single(userWithRole!.Roles);

            // revoke role
            userWithRole.RevokeRole(role);
            await _databaseFixture.DbContext.SaveChangesAsync();

            var userWithoutRole = await _userRepository.GetByIdAsync(user.Id);
            Assert.Empty(userWithoutRole!.Roles);
        }

        [Fact]
        public async Task PersistUser_ManagePermissions()
        {
            // Arrange
            var permission = Permission.Create("read:documents", "").Data!;
            var user = User.Create("johndoe", "johndoe@test.com", "hashedpassword", "John", "Doe").Data!;

            // Create user
            await _userRepository.AddAsync(user);
            await _permissionRepository.AddAsync(permission);
            await _databaseFixture.DbContext.SaveChangesAsync();

            // Get user
            var retrievedUser = await _userRepository.GetByIdAsync(user.Id);

            // assign permission
            retrievedUser.AddPermissions(permission);
            await _databaseFixture.DbContext.SaveChangesAsync();

            var userWithPermission = await _userRepository.GetByIdAsync(retrievedUser.Id);
            Assert.Single(userWithPermission!.Permissions);

            // remove permission
            userWithPermission.RemovePermission(permission);
            await _databaseFixture.DbContext.SaveChangesAsync();

            var userWithoutPermission = await _userRepository.GetByIdAsync(userWithPermission.Id);
            Assert.Empty(userWithoutPermission!.Permissions);
        }
    }
}
