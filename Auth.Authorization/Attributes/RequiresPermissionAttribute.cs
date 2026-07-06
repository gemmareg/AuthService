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
        public string Permission { get; }

        public RequiresPermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}
