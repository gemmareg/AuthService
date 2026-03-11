using AuthService.Application.Dtos;
using AuthService.Application.Features.Tokens.Queries.RefreshToken;
using AuthService.Application.Features.Users.Commands.CreateUser;
using AuthService.Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthService.Host.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController(IMediator mediator, ILogger<UserController> logger) : ControllerBase
    {
        [HttpPost("Refresh")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> CreateUser(RefreshTokenQuery command)
        {
            logger.LogInformation("Received RefreshTokenQuery: {@Command}", command);

            var result = await mediator.Send(command);

            return result.ToActionResult();
        }
    }
}
