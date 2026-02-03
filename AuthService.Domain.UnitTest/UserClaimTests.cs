namespace AuthService.Domain.UnitTest
{
    public class UserClaimTests
    {
        [Fact]
        public void Create_Should_Create_UserClaim_With_Valid_Ids()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claimId = Guid.NewGuid();

            // Act
            var userClaim = UserPermission.Create(userId, claimId);

            // Assert
            Assert.NotNull(userClaim);
            Assert.Equal(userId, userClaim.UserId);
            Assert.Equal(claimId, userClaim.ClaimId);
        }

        [Fact]
        public void Create_Should_Not_Set_Navigation_Properties()
        {
            var userClaim = UserPermission.Create(Guid.NewGuid(), Guid.NewGuid());

            Assert.Null(userClaim.User);
            Assert.Null(userClaim.Claim);
        }
    }
}
