using Auth.Authorization.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Authorization.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra el <see cref="PermissionFilter"/> como filtro MVC global, para
        /// que las acciones decoradas con <see cref="Attributes.RequiresPermissionAttribute"/>
        /// se autoricen automáticamente. No sustituye a AddControllers(): solo añade
        /// configuración adicional a MvcOptions, así que puede llamarse antes o
        /// después de él.
        /// </summary>
        public static IServiceCollection AddAuthAuthorization(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options => options.Filters.Add<PermissionFilter>());

            return services;
        }
    }
}
