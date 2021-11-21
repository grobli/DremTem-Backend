using System.Security.Cryptography;
using System.Text;

namespace DeviceManager.Core
{
    public static class KeyGenerator
    {
        private static readonly char[] Chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_".ToCharArray();

        public static string GetUniqueKey(int size)
        {
            var data = new byte[size];
            using var crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(data);
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(Chars[b % Chars.Length]);
            }

            return result.ToString();
        }
    }
}