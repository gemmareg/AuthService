namespace Auth.Contracts.UnitTest.Extensions
{
    using Auth.Contracts;
    using Auth.Contracts.Extensions;
    using System.Security.Claims;
    using Xunit;

    public class UserExtensionsTests
    {
        private ClaimsPrincipal CreateUser(
            string role = null,
            params string[] permissions)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            if (permissions != null)
            {
                claims.AddRange(
                    permissions.Select(p => new Claim(AuthClaimTypes.Permission, p))
                );
            }

            var identity = new ClaimsIdentity(claims, "test");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public void HasPermission_UserIsNull_ReturnsFalse()
        {
            ClaimsPrincipal user = null;

            var result = user.HasPermission("user.delete");

            Assert.False(result);
        }

        [Fact]
        public void HasPermission_UserIsAdmin_ReturnsTrue()
        {
            var user = CreateUser(role: "Admin");

            var result = user.HasPermission("anything.whatever");

            Assert.True(result);
        }

        [Fact]
        public void HasPermission_UserHasPermission_ReturnsTrue()
        {
            var user = CreateUser(permissions: new[] { "user.delete" });

            var result = user.HasPermission("user.delete");

            Assert.True(result);
        }

        [Fact]
        public void HasPermission_UserDoesNotHavePermission_ReturnsFalse()
        {
            var user = CreateUser(permissions: new[] { "user.read" });

            var result = user.HasPermission("user.delete");

            Assert.False(result);
        }

        [Fact]
        public void HasPermission_IsCaseInsensitive_ReturnsTrue()
        {
            var user = CreateUser(permissions: new[] { "USER.DELETE" });

            var result = user.HasPermission("user.delete");

            Assert.True(result);
        }

        [Fact]
        public void HasPermission_MultiplePermissions_OneMatches_ReturnsTrue()
        {
            var user = CreateUser(permissions: new[] { "user.read", "user.delete" });

            var result = user.HasPermission("user.delete");

            Assert.True(result);
        }
    }
}
