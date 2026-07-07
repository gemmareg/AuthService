namespace Auth.Contracts.Events
{
    /// <summary>
    /// Publicado cuando un usuario cambia su email
    /// (routing key "user.email-changed" en el exchange "auth.events").
    /// </summary>
    /// <param name="userId">Id del usuario.</param>
    /// <param name="email">Nuevo email del usuario.</param>
    public record EmailChangedEvent(string userId, string email);
}
