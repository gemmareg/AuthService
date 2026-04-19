namespace AuthService.Shared
{
    public static class ValidationHelpers
    {
        public static bool BeAValidGuid(string? value) => Guid.TryParse(value, out _);
    }
}
