namespace AuthService.Application.Dtos
{
    public class AuthResponse
    {
        public string AccessToken { get; init; } = default!;
        public string RefreshToken { get; init; } = default!;
        public DateTime? RefreshTokenExpiration { get; init; }
    }
}
