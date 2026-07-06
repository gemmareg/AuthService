using System.Security.Claims;

namespace Auth.Contracts.Extensions
{
    /// <summary>
    /// Extensiones sobre <see cref="ClaimsPrincipal"/> para leer los claims que
    /// AuthService emite en sus tokens. Cualquier servicio que valide tokens
    /// generados por AuthService (esta API u otras que confíen en el mismo JWT)
    /// puede reutilizar esta clase en lugar de re-implementar la lectura de claims.
    /// </summary>
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

        /// <summary>
        /// Comprueba si el usuario tiene el permiso indicado, entre los permisos
        /// efectivos incluidos en el token (directos + heredados de sus roles).
        /// NOTA: los usuarios con rol "Admin" pasan cualquier check, sin mirar si
        /// el permiso concreto está en la lista. Es una decisión de diseño explícita
        /// (patrón "superadmin"); si no la quieres, quita este bypass.
        /// </summary>
        public static bool HasPermission(this ClaimsPrincipal user, string requiredPermission)
        {
            if (user == null) return false;

            if (user.IsInRole("Admin"))
                return true;

            return user.FindAll(AuthClaimTypes.Permission)
                .Any(c => c.Value.Equals(requiredPermission, StringComparison.OrdinalIgnoreCase));
        }
    }
}
