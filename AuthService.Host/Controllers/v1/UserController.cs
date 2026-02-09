using AuthService.Application.Dtos;
using AuthService.Application.Features.Users.Commands.CreateUser;
using AuthService.Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthService.Host.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> CreateUser(CreateUserCommand command)
        {
            var result = await mediator.Send(command);

            return result.ToActionResult();
        }
    }
}
