namespace Auth.Contracts.Events;

public record AdminCreatedEvent(string userId, string email, string username);
