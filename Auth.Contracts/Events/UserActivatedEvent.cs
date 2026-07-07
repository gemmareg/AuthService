namespace Auth.Contracts.Events;

/// <summary>
/// Publicado cuando una cuenta inactiva vuelve a activarse (por ejemplo, al
/// hacer login después de haber sido desactivada por otro usuario)
/// (routing key "user.activated" en el exchange "auth.events").
/// </summary>
/// <param name="userId">Id del usuario reactivado.</param>
public record UserActivatedEvent(string userId);
