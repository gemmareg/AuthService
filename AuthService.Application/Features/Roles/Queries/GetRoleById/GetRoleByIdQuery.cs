using AuthService.Application.Dtos;
using AuthService.Shared.Result.Generic;
using MediatR;

namespace AuthService.Application.Features.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQuery : IRequest<Result<RoleResponse>>
    {
        public Guid RoleId { get; set; }
    }
}
