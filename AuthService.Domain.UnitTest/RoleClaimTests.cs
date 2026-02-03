namespace AuthService.Domain.UnitTest
{
    public class RoleClaimTests
    {
        [Fact]
        public void Create_Should_Create_RoleClaim_With_Valid_Ids()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var claimId = Guid.NewGuid();

            // Act
            var roleClaim = RoleClaim.Create(roleId, claimId);

            // Assert
            Assert.NotNull(roleClaim);
            Assert.Equal(roleId, roleClaim.RoleId);
            Assert.Equal(claimId, roleClaim.ClaimId);
        }

        [Fact]
        public void Create_Should_Not_Set_Navigation_Properties()
        {
            var roleClaim = RoleClaim.Create(Guid.NewGuid(), Guid.NewGuid());

            Assert.Null(roleClaim.Role);
            Assert.Null(roleClaim.Claim);
        }
    }
}
