using AuthService.Shared;

namespace AuthService.Domain.UnitTest
{
    public class ClaimTests
    {
        [Fact]
        public void Create_Should_Create_Claim_When_Data_Is_Valid()
        {
            var result = Claim.Create("permission", "read:orders");

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("permission", result.Data!.Type);
            Assert.Equal("read:orders", result.Data!.Value);
        }

        [Fact]
        public void Create_Should_Fail_When_Type_Is_Empty()
        {
            var result = Claim.Create("", "read");

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.TYPE_CLAIM_NOT_NULL, result.Message);
        }
    }
}
