using AuthService.Application.Dtos;
using AuthService.Application.Features.Users.Commands.CreateUser;
using AuthService.Application.Features.Users.Commands.LoginUser;
using AuthService.Application.Features.Users.Commands.SoftDeleteUser;
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SoftDelete(Guid userId)
        {
            logger.LogInformation("Received SoftDeleteUserCommand for userId: {UserId}", userId);

            var result = await mediator.Send(new SoftDeleteUserCommand { UserId = userId });

            return result.ToActionResult();
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            var claims = User.Identities.First().Claims;
            var userId = claims.FirstOrDefault(e => e.ToString().Contains("nameidentifier"))!.ToString().Split(" ")[1];
            var email = claims.FirstOrDefault(e => e.ToString().Contains("emailaddress"))!.ToString().Split(" ")[1];
            var roles = claims
                .Where(e => e.ToString().Contains("role"))
                .Select(e => e.ToString().Split(" ")[1])
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
