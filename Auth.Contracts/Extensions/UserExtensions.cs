using System.Security.Claims;

namespace AuthService.Contracts.Extensions
{
    public static class UserExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value
                ?? user?.FindFirst("id")?.Value ?? string.Empty;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Email)?.Value
                ?? user?.FindFirst("email")?.Value ?? string.Empty;
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Name)?.Value
                ?? user?.FindFirst("username")?.Value ?? string.Empty;
        }

        public static List<string> GetRoles(this ClaimsPrincipal user)
        {
            return user?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? user?.FindAll("role").Select(c => c.Value).ToList() ?? new List<string>();
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user?.IsInRole("Admin") ?? false;
        }

        public static bool HasPermission(this ClaimsPrincipal user, string requiredPermission)
        {
            if (user == null) return false;

            // bypass para admin
            if (user.IsInRole("Admin"))
                return true;

            return user.FindAll("permissions")
                .Any(c => c.Value.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase));
        }
    }
}
