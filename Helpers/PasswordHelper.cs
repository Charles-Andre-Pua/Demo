using System.Security.Cryptography;
using System.Text;

namespace Demo.Helpers
{
    public static class PasswordHelper
    {
        private static readonly Random _rng = new Random();

        public static string GenerateSalt(int length = 5)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_rng.Next(s.Length)]).ToArray());
        }

        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var salted = password + salt;
                var bytes = Encoding.UTF8.GetBytes(salted);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }

}
