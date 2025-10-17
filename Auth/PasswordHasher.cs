using System.Security.Cryptography;
using System.Text;

namespace bank_api.Auth
{
    public class PasswordHasher
    {
        private const int SaltSizeBytes = 16; // 128-bit
        private const int HashSizeBytes = 32; // 256-bit
        private const int DefaultIterations = 100_000;

        // Format: v1.iterations.base64Salt.base64Hash
        private const string FormatVersion = "v1";

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password must not be empty.", nameof(password));
            }

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                DefaultIterations,
                HashAlgorithmName.SHA256,
                HashSizeBytes
            );

            string encoded = string.Join('.',
                FormatVersion,
                DefaultIterations.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash)
            );

            return encoded;
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            string[] parts = storedHash.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4 || parts[0] != FormatVersion)
            {
                return false;
            }

            if (!int.TryParse(parts[1], out int iterations))
            {
                return false;
            }

            byte[] salt;
            byte[] expectedHash;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch
            {
                return false;
            }

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length
            );

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
