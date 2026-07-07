namespace Auth.Contracts.Events;

/// <summary>
/// Publicado cuando <c>AuthDbSeeder</c> crea (o encuentra ya existente, en
/// cada arranque) la cuenta administradora determinista definida por
/// configuración (routing key "admin.created" en el exchange "auth.events").
/// </summary>
/// <param name="userId">Id del usuario administrador (fijado por configuración, estable entre entornos).</param>
/// <param name="email">Email del usuario administrador.</param>
/// <param name="username">Username del usuario administrador.</param>
public record AdminCreatedEvent(string userId, string email, string username);
