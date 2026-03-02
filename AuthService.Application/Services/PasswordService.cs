using AuthService.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace AuthService.Application.Services
{
    public class PasswordService(ILogger<PasswordService> logger) : IPasswordService
    {
        private const int SaltSize = 16;      // 128 bits
        private const int KeySize = 32;       // 256 bits
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public string Hash(string plainPassword)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                plainPassword,
                salt,
                Iterations,
                Algorithm,
                KeySize
            );

            logger.LogDebug("Generated password hash using {Iterations} iterations", Iterations);

            // Guardamos todo junto: iterations.salt.hash
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verify(string plainPassword, string passwordHash)
        {
            var parts = passwordHash.Split('.', 3);
            if (parts.Length != 3) return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                plainPassword,
                salt,
                iterations,
                Algorithm,
                hash.Length
            );

            var result = CryptographicOperations.FixedTimeEquals(inputHash, hash);
            if (!result) logger.LogWarning("Password verification failed");

            return result;
        }
    }
}
