using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Authorization.Attributes
{
    /// <summary>
    /// Debe implementar <see cref="IFilterMetadata"/> (interfaz marcador, sin
    /// miembros): así es como el pipeline de MVC descubre automáticamente que
    /// este atributo participa como filtro y lo añade a
    /// AuthorizationFilterContext.Filters. Sin esto, <see cref="Filters.PermissionFilter"/>
    /// nunca lo encontraría y la comprobación de permisos no tendría efecto.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequiresPermissionAttribute : Attribute, IFilterMetadata
    {
        /// <summary>Nombre del permiso requerido (p. ej. <c>"roles:read"</c>). Ver <see cref="Auth.Contracts.AuthPermissions"/>.</summary>
        public string Permission { get; }

        /// <param name="permission">Nombre del permiso requerido para acceder a la acción/controller decorado.</param>
        public RequiresPermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}
