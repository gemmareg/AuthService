using AuthService.Shared.Result.Generic;
using AuthService.Shared.Constants;
using AuthService.Shared.Result.NonGeneric;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Host.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.Success) return new OkObjectResult(result.Data);
            if (result.Message == UserErrorMessages.AccountDeactivatedByAdmin || result.Message == UserErrorMessages.SoftDeleteForbidden)
                return new ForbidResult();

            return new BadRequestObjectResult(result.Message) { };
        }

        public static ActionResult ToActionResult(this Result result)
        {
            if (result.Success) return new OkObjectResult(result);
            if (result.Message == UserErrorMessages.SoftDeleteForbidden)
                return new ForbidResult();

            return new BadRequestObjectResult(result.Message) { };
        }
    }
}
