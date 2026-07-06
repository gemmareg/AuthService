using AuthService.Shared;

namespace AuthService.Domain.UnitTest
{
    public class RoleTests
    {
        [Fact]
        public void Create_Should_Create_Role_When_Name_Is_Valid()
        {
            var result = Role.Create("admin");

            Assert.True(result.Success);
            Assert.Equal("admin", result.Data!.Name);
        }

        [Fact]
        public void Create_Should_Fail_When_Name_Is_Empty()
        {
            var result = Role.Create("");

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.ROLE_NAME_NOT_NULL, result.Message);
        }

        [Fact]
        public void AddPermission_Should_Add_Permission_When_Not_Exists()
        {
            var role = Role.Create("admin").Data!;
            var permission = Permission.Create("permission", "read:users").Data!;

            var result = role.AddPermission(permission);

            Assert.True(result.Success);
            Assert.Contains(permission, role.Permissions);
        }

        [Fact]
        public void AddPermission_Should_Fail_When_Permission_Already_Exists()
        {
            var role = Role.Create("admin").Data!;
            var permission = Permission.Create("permission", "read:users").Data!;

            role.AddPermission(permission);
            var result = role.AddPermission(permission);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.PERMISSION_ALREADY_ASSIGNED, result.Message);
        }

        [Fact]
        public void Rename_Should_Update_Name_When_Valid()
        {
            var role = Role.Create("admin").Data!;

            var result = role.Rename("superadmin");

            Assert.True(result.Success);
            Assert.Equal("superadmin", role.Name);
        }

        [Fact]
        public void Rename_Should_Fail_When_Name_Is_Empty()
        {
            var role = Role.Create("admin").Data!;

            var result = role.Rename("");

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.ROLE_NAME_NOT_NULL, result.Message);
            Assert.Equal("admin", role.Name);
        }

        [Fact]
        public void RemovePermission_Should_Remove_Permission_When_Assigned()
        {
            var role = Role.Create("admin").Data!;
            var permission = Permission.Create("permission", "read:users").Data!;
            role.AddPermission(permission);

            var result = role.RemovePermission(permission);

            Assert.True(result.Success);
            Assert.DoesNotContain(permission, role.Permissions);
        }

        [Fact]
        public void RemovePermission_Should_Fail_When_Not_Assigned()
        {
            var role = Role.Create("admin").Data!;
            var permission = Permission.Create("permission", "read:users").Data!;

            var result = role.RemovePermission(permission);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.PERMISSION_NOT_ASSIGNED, result.Message);
        }
    }
}
