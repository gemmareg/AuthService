namespace AuthService.Domain.Extensions
{
    /// <summary>
    /// Calcula los permisos "efectivos" de un usuario: sus permisos directos
    /// más los heredados de sus roles, deduplicados. Es la misma lógica que se
    /// usa para construir los claims del token (<see cref="AuthService.Application.Services.TokenService"/>)
    /// y para validar permisos contra el estado actual en base de datos
    /// (por ejemplo, en <see cref="AuthService.Application.Services.UserService"/>),
    /// así que vive aquí una única vez en vez de duplicarse en cada sitio.
    /// </summary>
    public static class UserPermissionExtensions
    {
        public static IEnumerable<string> GetEffectivePermissions(this User user)
            => user.Permissions
                .Select(p => p.Name)
                .Concat(user.Roles.SelectMany(r => r.Permissions.Select(p => p.Name)))
                .Distinct(StringComparer.OrdinalIgnoreCase);

        public static bool HasEffectivePermission(this User user, string permission)
            => user.GetEffectivePermissions()
                .Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase));
    }
}
