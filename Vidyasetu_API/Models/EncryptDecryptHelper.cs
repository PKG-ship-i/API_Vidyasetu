using System.Text;

namespace Vidyasetu_API.Models
{
    public static class EncryptDecryptHelper
    {
        public static string Encrypt(string requestId, string deviceId)
        {
            string combined = $"{requestId}:{deviceId}";
            byte[] bytes = Encoding.UTF8.GetBytes(combined);
            return Convert.ToBase64String(bytes);
        }

        public static (string requestId, string deviceId) Decrypt(string encrypted)
        {
            byte[] bytes = Convert.FromBase64String(encrypted);
            string result = Encoding.UTF8.GetString(bytes);

            var parts = result.Split(':');
            if (parts.Length != 2)
                throw new InvalidOperationException("Invalid token format.");

            return (parts[0], parts[1]);
        }
    }
}
