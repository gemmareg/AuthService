namespace Auth.Contracts.Events;

/// <summary>
/// Publicado cuando un usuario se desactiva (soft-delete), ya sea por sí
/// mismo o por otro usuario con el permiso correspondiente
/// (routing key "user.soft-deleted" en el exchange "auth.events").
/// </summary>
/// <param name="userId">Id del usuario desactivado.</param>
public record UserSoftDeletedEvent(string userId);
