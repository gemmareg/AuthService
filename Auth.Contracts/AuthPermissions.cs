namespace Auth.Contracts
{
    /// <summary>
    /// Nombres de permisos "bien conocidos" que varios endpoints de AuthService
    /// (y potencialmente otros servicios que confíen en sus tokens) necesitan
    /// referenciar. Centralizarlos aquí evita magic strings duplicados y typos
    /// entre el controller, el seeder y cualquier consumidor externo.
    ///
    /// Convención: "recurso:accion" y, cuando aplica, ":any" para distinguir
    /// una acción sobre cualquier recurso de la acción equivalente sobre el
    /// propio recurso del usuario autenticado (que normalmente no requiere permiso).
    /// </summary>
    public static class AuthPermissions
    {
        /// <summary>Permite desactivar (soft-delete) la cuenta de otro usuario.</summary>
        public const string UsersDeleteAny = "users:delete:any";

        /// <summary>Permite editar los datos de otro usuario.</summary>
        public const string UsersUpdateAny = "users:update:any";
    }
}
