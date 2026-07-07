using Auth.Authorization.Attributes;
using Auth.Contracts.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Authorization.Filters
{
    /// <summary>
    /// Filtro de autorización de MVC que aplica <see cref="RequiresPermissionAttribute"/>.
    /// Se registra automáticamente al llamar a
    /// <see cref="Extensions.ServiceCollectionExtensions.AddAuthAuthorization"/>; no hace
    /// falta instanciarlo ni registrarlo a mano.
    /// </summary>
    public class PermissionFilter : IAuthorizationFilter
    {
        /// <summary>
        /// Busca un <see cref="RequiresPermissionAttribute"/> en la acción/controller
        /// actual y, si existe, comprueba con <see cref="UserExtensions.HasPermission"/>
        /// que el usuario autenticado de la petición lo tenga. Si no hay atributo, no
        /// hace nada (la acción queda solo protegida por lo que ya exija [Authorize]).
        /// Si el usuario no tiene el permiso, corta la petición con 403 Forbidden.
        /// </summary>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var attribute = context.Filters
                .OfType<RequiresPermissionAttribute>()
                .FirstOrDefault();

            if (attribute == null)
                return;

            var user = context.HttpContext.User;

            if (!user.HasPermission(attribute.Permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
