using Auth.Authorization.Attributes;
using Auth.Contracts;
using AuthService.Application.Dtos;
using AuthService.Application.Features.Permissions.Commands.ActivatePermission;
using AuthService.Application.Features.Permissions.Commands.CreatePermission;
using AuthService.Application.Features.Permissions.Commands.DeactivatePermission;
using AuthService.Application.Features.Permissions.Commands.DeletePermission;
using AuthService.Application.Features.Permissions.Commands.UpdatePermissionDescription;
using AuthService.Application.Features.Permissions.Queries.GetAllPermissions;
using AuthService.Application.Features.Permissions.Queries.GetPermissionById;
using AuthService.Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Host.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController(IMediator mediator, ILogger<PermissionController> logger) : ControllerBase
    {
        [RequiresPermission(AuthPermissions.PermissionsCreate)]
        [HttpPost]
        [ProducesResponseType(typeof(PermissionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PermissionResponse>> Create(CreatePermissionRequest request)
        {
            logger.LogInformation("Received CreatePermissionCommand for name: {Name}", request.Name);

            var result = await mediator.Send(new CreatePermissionCommand { Name = request.Name, Description = request.Description });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.PermissionsRead)]
        [HttpGet("{permissionId:guid}")]
        [ProducesResponseType(typeof(PermissionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PermissionResponse>> GetById(Guid permissionId)
        {
            var result = await mediator.Send(new GetPermissionByIdQuery { PermissionId = permissionId });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.PermissionsRead)]
        [HttpGet]
        [ProducesResponseType(typeof(List<PermissionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<PermissionResponse>>> GetAll()
        {
            var result = await mediator.Send(new GetAllPermissionsQuery());

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.PermissionsUpdate)]
        [HttpPut("{permissionId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateDescription(Guid permissionId, UpdatePermissionDescriptionRequest request)
        {
            logger.LogInformation("Received UpdatePermissionDescriptionCommand for permissionId: {PermissionId}", permissionId);

            var result = await mediator.Send(new UpdatePermissionDescriptionCommand { PermissionId = permissionId, Description = request.Description });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.PermissionsUpdate)]
        [HttpPost("{permissionId:guid}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Activate(Guid permissionId)
        {
            var result = await mediator.Send(new ActivatePermissionCommand { PermissionId = permissionId });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.PermissionsUpdate)]
        [HttpPost("{permissionId:guid}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Deactivate(Guid permissionId)
        {
            var result = await mediator.Send(new DeactivatePermissionCommand { PermissionId = permissionId });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.PermissionsDelete)]
        [HttpDelete("{permissionId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Delete(Guid permissionId)
        {
            logger.LogInformation("Received DeletePermissionCommand for permissionId: {PermissionId}", permissionId);

            var result = await mediator.Send(new DeletePermissionCommand { PermissionId = permissionId });

            return result.ToActionResult();
        }
    }
}
