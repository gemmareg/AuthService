using System.Reflection;
using AuthService.Application.Abstractions.Security;

namespace AuthService.Application.Security
{
    /// <summary>
    /// Comprueba contraseñas contra una lista embebida de contraseñas comunes
    /// o filtradas conocidas (Security/CommonPasswords.txt, empaquetada como
    /// recurso embebido). Sin dependencias externas: no llama a ningún servicio
    /// de terceros, así que funciona igual con o sin acceso a internet.
    /// </summary>
    public class CommonPasswordChecker : ICommonPasswordChecker
    {
        private static readonly Lazy<HashSet<string>> CommonPasswords = new(LoadCommonPasswords);

        public bool IsCommon(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            return CommonPasswords.Value.Contains(password.Trim().ToLowerInvariant());
        }

        private static HashSet<string> LoadCommonPasswords()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("CommonPasswords.txt", StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
            {
                return new HashSet<string>();
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                return new HashSet<string>();
            }

            using var reader = new StreamReader(stream);
            var passwords = new HashSet<string>(StringComparer.Ordinal);

            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 0)
                {
                    passwords.Add(trimmed.ToLowerInvariant());
                }
            }

            return passwords;
        }
    }
}
