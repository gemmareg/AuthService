using Auth.Authorization.Attributes;
using Auth.Contracts;
using AuthService.Application.Dtos;
using AuthService.Application.Features.Roles.Commands.AssignPermissionToRole;
using AuthService.Application.Features.Roles.Commands.CreateRole;
using AuthService.Application.Features.Roles.Commands.DeleteRole;
using AuthService.Application.Features.Roles.Commands.RemovePermissionFromRole;
using AuthService.Application.Features.Roles.Commands.RenameRole;
using AuthService.Application.Features.Roles.Queries.GetAllRoles;
using AuthService.Application.Features.Roles.Queries.GetRoleById;
using AuthService.Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Host.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController(IMediator mediator, ILogger<RoleController> logger) : ControllerBase
    {
        [RequiresPermission(AuthPermissions.RolesCreate)]
        [HttpPost]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RoleResponse>> Create(CreateRoleRequest request)
        {
            logger.LogInformation("Received CreateRoleCommand for name: {Name}", request.Name);

            var result = await mediator.Send(new CreateRoleCommand { Name = request.Name });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.RolesRead)]
        [HttpGet("{roleId:guid}")]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RoleResponse>> GetById(Guid roleId)
        {
            var result = await mediator.Send(new GetRoleByIdQuery { RoleId = roleId });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.RolesRead)]
        [HttpGet]
        [ProducesResponseType(typeof(List<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<RoleResponse>>> GetAll()
        {
            var result = await mediator.Send(new GetAllRolesQuery());

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.RolesUpdate)]
        [HttpPut("{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Rename(Guid roleId, RenameRoleRequest request)
        {
            logger.LogInformation("Received RenameRoleCommand for roleId: {RoleId}", roleId);

            var result = await mediator.Send(new RenameRoleCommand { RoleId = roleId, Name = request.Name });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.RolesDelete)]
        [HttpDelete("{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Delete(Guid roleId)
        {
            logger.LogInformation("Received DeleteRoleCommand for roleId: {RoleId}", roleId);

            var result = await mediator.Send(new DeleteRoleCommand { RoleId = roleId });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.RolesUpdate)]
        [HttpPost("{roleId:guid}/permissions/{permissionId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignPermission(Guid roleId, Guid permissionId)
        {
            var result = await mediator.Send(new AssignPermissionToRoleCommand { RoleId = roleId, PermissionId = permissionId });

            return result.ToActionResult();
        }

        [RequiresPermission(AuthPermissions.RolesUpdate)]
        [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RemovePermission(Guid roleId, Guid permissionId)
        {
            var result = await mediator.Send(new RemovePermissionFromRoleCommand { RoleId = roleId, PermissionId = permissionId });

            return result.ToActionResult();
        }
    }
}
