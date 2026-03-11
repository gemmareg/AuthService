namespace Auth.Contracts.Events
{
    public class UserSoftDeletedEvent
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
