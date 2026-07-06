using AuthService.Application.Security;

namespace AuthService.Application.UnitTest.Security
{
    public class CommonPasswordCheckerTests
    {
        private readonly CommonPasswordChecker _checker = new();

        [Theory]
        [InlineData("123456")]
        [InlineData("password")]
        [InlineData("Password1")]
        [InlineData("QWERTY123")]
        public void IsCommon_Should_Return_True_For_Known_Common_Passwords(string password)
        {
            Assert.True(_checker.IsCommon(password));
        }

        [Fact]
        public void IsCommon_Should_Return_False_For_Strong_Uncommon_Password()
        {
            Assert.False(_checker.IsCommon("Xk9#mQ2vT!pL7zR4"));
        }

        [Fact]
        public void IsCommon_Should_Return_False_For_Empty_Password()
        {
            Assert.False(_checker.IsCommon(""));
        }
    }
}
