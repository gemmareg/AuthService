namespace AuthService.Application.Dtos.Events
{
    public class UserRegisteredEvent
    {
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
