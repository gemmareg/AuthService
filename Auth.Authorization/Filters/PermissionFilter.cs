using Auth.Contracts.Attributes;
using Auth.Contracts.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Contracts.Filters
{
    public class PermissionFilter : IAuthorizationFilter
    {
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
