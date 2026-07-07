namespace Auth.Contracts.Events;

/// <summary>
/// Publicado cuando un usuario se registra correctamente en AuthService
/// (routing key "user.registered" en el exchange "auth.events").
/// </summary>
/// <param name="userId">Id del usuario recién creado.</param>
/// <param name="name">Nombre del usuario.</param>
public record UserRegisteredEvent(string userId, string name);
