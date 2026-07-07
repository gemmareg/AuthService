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
        /// <summary>
        /// Id del usuario (claim "nameidentifier"/"sub"/"id", en ese orden de
        /// preferencia). Devuelve <see cref="string.Empty"/> si el principal es
        /// nulo o no lleva ninguno de esos claims.
        /// </summary>
        public static string GetId(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value
                ?? user?.FindFirst("id")?.Value ?? string.Empty;
        }

        /// <summary>
        /// Email del usuario (claim "emailaddress"/"email"). Devuelve
        /// <see cref="string.Empty"/> si el principal es nulo o no lleva el claim.
        /// </summary>
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Email)?.Value
                ?? user?.FindFirst("email")?.Value ?? string.Empty;
        }

        /// <summary>
        /// Username del usuario (claim "name"/"username"). Devuelve
        /// <see cref="string.Empty"/> si el principal es nulo o no lleva el claim.
        /// </summary>
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Name)?.Value
                ?? user?.FindFirst("username")?.Value ?? string.Empty;
        }

        /// <summary>
        /// Lista de nombres de rol del usuario (claim "role", uno por cada rol
        /// asignado). Devuelve una lista vacía si el principal es nulo o no
        /// tiene ningún rol.
        /// </summary>
        public static List<string> GetRoles(this ClaimsPrincipal user)
        {
            return user?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                ?? user?.FindAll("role").Select(c => c.Value).ToList() ?? new List<string>();
        }

        /// <summary>
        /// True si el usuario tiene el rol "Admin". Atajo equivalente a
        /// <c>user.GetRoles().Contains("Admin")</c>, pero usando
        /// <see cref="ClaimsPrincipal.IsInRole(string)"/> directamente.
        /// </summary>
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
