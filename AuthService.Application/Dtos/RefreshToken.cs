namespace AuthService.Application.Dtos
{
    public class RefreshToken
    {
        public string Token { get; init; } = default!;
        public DateTime Expiration { get; init; }
    }
}
