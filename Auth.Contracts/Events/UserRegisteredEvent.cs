namespace Auth.Contracts.Events
{
    public class UserRegisteredEvent
    {
        public string UserId { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
