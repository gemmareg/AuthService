using Auth.Contracts;
using Auth.Contracts.Extensions;
using AuthService.Application.Dtos;
using AuthService.Application.Features.Users.Commands.CreateUser;
using AuthService.Application.Features.Users.Commands.LoginUser;
using AuthService.Application.Features.Users.Commands.SoftDeleteUser;
using AuthService.Application.Features.Users.Commands.UpdateUser;
using AuthService.Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthService.Host.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IMediator mediator, ILogger<UserController> logger) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> CreateUser(CreateUserCommand command)
        {
            logger.LogInformation("Received CreateUserCommand: {@Command}", command);

            var result = await mediator.Send(command);

            return result.ToActionResult();
        }

        [Authorize]
        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SoftDelete(Guid userId)
        {
            var requesterIdClaim = User.GetId();
            if (!Guid.TryParse(requesterIdClaim, out var requesterId))
            {
                logger.LogWarning("Soft delete denied due to invalid requester id claim. Claim value: {ClaimValue}", requesterIdClaim);
                return Unauthorized("Invalid authentication context");
            }

            // Cualquier usuario puede desactivar su propia cuenta. Desactivar la
            // cuenta de otro requiere el permiso "users:delete:any" (comprobado
            // aquí contra el token para responder rápido; UserService repite la
            // comprobación contra el estado actual en BD como red de seguridad).
            if (requesterId != userId && !User.HasPermission(AuthPermissions.UsersDeleteAny))
            {
                logger.LogWarning("Soft delete forbidden. Requester {RequesterId} lacks {Permission} to deactivate user {UserId}", requesterId, AuthPermissions.UsersDeleteAny, userId);
                return Forbid();
            }

            logger.LogInformation("Received SoftDeleteUserCommand for userId: {UserId}", userId);

            var result = await mediator.Send(new SoftDeleteUserCommand
            {
                UserId = userId,
                RequesterId = requesterId
            });

            return result.ToActionResult();
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserCommand command)
        {
            logger.LogInformation("Received LoginUserCommand for email: {Email}", command.Email);

            var result = await mediator.Send(command);

            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("authentication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult GetAuthentication()
        {
            var userId = User.GetId();
            var email = User.GetEmail();
            var roles = User.GetRoles();

            return Ok(new
            {
                Authenticated = User.Identity?.IsAuthenticated ?? false,
                UserId = userId,
                Email = email,
                Roles = roles
            });
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(UpdateUserRequest request)
        {
            var requesterIdClaim = User.GetId();
            if (!Guid.TryParse(requesterIdClaim, out var requesterId))
            {
                logger.LogWarning("Update denied due to invalid requester id claim. Claim value: {ClaimValue}", requesterIdClaim);
                return Unauthorized("Invalid authentication context");
            }

            // Por defecto se actualiza el propio usuario. Solo se permite
            // apuntar a otro TargetUserId si el llamante tiene permiso explícito;
            // el Id nunca decide por sí solo a quién se edita (evita IDOR).
            var targetUserId = request.TargetUserId ?? requesterId;
            if (targetUserId != requesterId && !User.HasPermission(AuthPermissions.UsersUpdateAny))
            {
                logger.LogWarning("Update forbidden. Requester {RequesterId} lacks {Permission} to update user {TargetUserId}", requesterId, AuthPermissions.UsersUpdateAny, targetUserId);
                return Forbid();
            }

            logger.LogInformation("Received UpdateUserCommand for userId: {UserId}", targetUserId);

            var result = await mediator.Send(new UpdateUserCommand
            {
                RequesterId = requesterId,
                TargetUserId = targetUserId,
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email
            });

            return result.ToActionResult();
        }
    }
}
