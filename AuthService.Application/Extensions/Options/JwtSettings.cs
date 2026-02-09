namespace AuthService.Application.Extensions.Options
{
    public class JwtSettings
    {
        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public string SecretKey { get; init; } = default!;
    }
}
