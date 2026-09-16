using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace UGB.MVC.Aplicaciones.Seguras.Helper
{
    public static class HashHelper
    {
        public static HashedPassword Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            return new HashedPassword
            {
                Password = hashed,
                Salt = Convert.ToBase64String(salt)
            };
        }

        public static bool CheckHash(string attemptedPassword, string hash, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: attemptedPassword,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(hash),
                Convert.FromBase64String(hashed));
        }
    }

    public class HashedPassword
    {
        public string Password { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }
}
