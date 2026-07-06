using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest<Result<RoleResponse>>
    {
        public string? Name { get; set; }
    }
}
