using AuthService.Application.Dtos;
using AuthService.Application.Features.Users.Commands.CreateUser;
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
        [HttpGet("authentication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult GetAuthentication()
        {
            var userId = User.FindFirst("sub")?.Value;
            var email = User.FindFirst("email")?.Value;
            var roles = User.Claims
                .Where(claim => claim.Type == "role")
                .Select(claim => claim.Value)
                .ToArray();

            return Ok(new
            {
                Authenticated = User.Identity?.IsAuthenticated ?? false,
                UserId = userId,
                Email = email,
                Roles = roles
            });
        }
    }
}
