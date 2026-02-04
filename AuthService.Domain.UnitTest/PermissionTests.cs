using AuthService.Shared;

namespace AuthService.Domain.UnitTest
{
    public class PermissionTests
    {
        [Fact]
        public void Create_Should_Create_Permission_When_Data_Is_Valid()
        {
            var result = Permission.Create("permission", "read:orders");

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("permission", result.Data!.Name);
            Assert.Equal("read:orders", result.Data!.Description);
        }

        [Fact]
        public void Create_Should_Fail_When_Type_Is_Empty()
        {
            var result = Permission.Create("", "read");

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.TYPE_PERMISSION_NOT_NULL, result.Message);
        }
    }
}
