using System.Security.Cryptography;
using System.Text;

namespace turfbooking.Helper
{
    public static class PasswordHelper
    {
        public static string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPasswordWithSHA256(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(salt + password);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedSaltedHash)
        {
            var parts = storedSaltedHash.Split(':');
            if (parts.Length != 2) return false;

            var salt = parts[0];
            var storedHash = parts[1];

            var enteredHash = HashPasswordWithSHA256(enteredPassword, salt);

            return storedHash == enteredHash;
        }
    }
}
