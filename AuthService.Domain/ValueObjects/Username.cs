using System.Text.RegularExpressions;

namespace AuthService.Domain.ValueObjects
{
    public sealed class Username
    {
        public string Value { get; }

        private Username(string value)
        {
            Value = value;
        }

        public static Username Create(string baseName, int number)
        {
            return new Username($"{baseName.Trim().ToLower()}{number}");
        }

        public static bool IsValid(string candidate, string baseName)
        {
            var pattern = $"^{Regex.Escape(baseName)}(\\d+)$";
            return Regex.IsMatch(candidate, pattern);
        }
    }
}
