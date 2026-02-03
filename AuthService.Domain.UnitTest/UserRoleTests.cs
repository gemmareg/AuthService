namespace AuthService.Domain.UnitTest
{
    public class UserRoleTests
    {
        [Fact]
        public void Create_Should_Create_UserRole_With_Valid_Ids()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            // Act
            var userRole = UserRole.Create(userId, roleId);

            // Assert
            Assert.NotNull(userRole);
            Assert.Equal(userId, userRole.UserId);
            Assert.Equal(roleId, userRole.RoleId);
        }

        [Fact]
        public void Create_Should_Not_Set_Navigation_Properties()
        {
            var userRole = UserRole.Create(Guid.NewGuid(), Guid.NewGuid());

            Assert.Null(userRole.User);
            Assert.Null(userRole.Role);
        }
    }
}
