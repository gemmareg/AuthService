namespace AuthService.Application.Dtos
{
    /// <summary>
    /// Body aceptado por PUT /api/user. Deliberadamente NO incluye el Id del
    /// usuario a modificar como obligatorio: por defecto se actualiza el propio
    /// usuario autenticado. <see cref="TargetUserId"/> es opcional y solo tiene
    /// efecto si el llamante tiene el permiso "users:update:any"; en caso
    /// contrario el controller lo rechaza con 403 antes de llegar aquí.
    /// </summary>
    public class UpdateUserRequest
    {
        public Guid? TargetUserId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
    }
}
