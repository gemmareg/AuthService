using AuthService.Shared.Result.NonGeneric;
using MediatR;

namespace AuthService.Application.Features.Users.Commands.UpdateUser
{
    /// <summary>
    /// Comando interno de MediatR. RequesterId y TargetUserId siempre los
    /// establece el controller a partir de los claims del token (nunca del
    /// body de la petición), para evitar que un usuario autenticado pueda
    /// editar la cuenta de otro simplemente indicando su Id (IDOR).
    /// </summary>
    public class UpdateUserCommand : IRequest<Result>
    {
        public Guid RequesterId { get; set; }
        public Guid TargetUserId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
    }
}
