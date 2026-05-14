using System.Security.Cryptography;
using System.Text;

namespace BankMore.Domain.Utils
{
    public static class SecurityUtils
    {
        public static string GerarHash(string senha, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedPassword = string.Concat(senha, salt);
                var bytes = Encoding.UTF8.GetBytes(combinedPassword);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}