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
        public void AddClaim_Should_Add_Claim_When_Not_Exists()
        {
            var role = Role.Create("admin").Data!;
            var claim = Claim.Create("permission", "read:users").Data!;
            var roleClaim = RoleClaim.Create(role.Id, claim.Id);

            var result = role.AddRoleClaim(roleClaim);

            Assert.True(result.Success);
            Assert.Contains(roleClaim, role.RoleClaims);
        }

        [Fact]
        public void AddClaim_Should_Fail_When_Claim_Already_Exists()
        {
            var role = Role.Create("admin").Data!;
            var claim = Claim.Create("permission", "read:users").Data!;
            var roleClaim = RoleClaim.Create(role.Id, claim.Id);

            role.AddRoleClaim(roleClaim);
            var result = role.AddRoleClaim(roleClaim);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.CLAIM_ALREADY_ASSIGNED, result.Message);
        }
    }
}
