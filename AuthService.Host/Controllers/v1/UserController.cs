using AuthService.Application.Dtos;
using AuthService.Application.Features.Users.Commands.CreateUser;
using AuthService.Application.Features.Users.Commands.LoginUser;
using AuthService.Application.Features.Users.Commands.SoftDeleteUser;
using AuthService.Application.Features.Users.Commands.UpdateUser;
using AuthService.Application.Features.Users.Queries.GetUser;
using AuthService.Host.Extensions;
using AuthService.Host.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

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
            logger.LogInformation("Received SoftDeleteUserCommand for userId: {UserId}", userId);

            var requesterIdClaim = User.GetId();
            if (!Guid.TryParse(requesterIdClaim, out var requesterId))
            {
                logger.LogWarning("Soft delete denied due to invalid requester id claim. Claim value: {ClaimValue}", requesterIdClaim);
                return Unauthorized("Invalid authentication context");
            }

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
            return Ok(new
            {
                Authenticated = User.Identity?.IsAuthenticated ?? false,
                UserId = User.GetId(),
                Email = User.GetEmail(),
                Roles = User.GetRoles()
            });
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponse>> UpdateUser(UpdateUserRequest request)
        {
            logger.LogInformation("Received RefreshTokenCommand for userId: {UserId}", request.Id);
            var command = new UpdateUserCommand
            {
                Id = request.Id,
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Password = request.Password,
                RequesterId = User.GetId()
            };  
            var result = await mediator.Send(command);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(GetUserByIdQueryResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetUserByIdQueryResponse>> GetUserById(string userId)
        {
            logger.LogInformation("Received GetUserByIdQuery for userId: {UserId}", userId);
            var query = new GetUserByIdQuery()
            {
                Id = userId
            };

            var result = await mediator.Send(query);

            return result.ToActionResult();
        }
    }
}
