using System;
using System.Security.Cryptography;
using System.Text;

namespace UserGrpcService.Utilities.Security
{
    public static class KeyGenerator
    {
        private static readonly char[] Chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_".ToCharArray();

        public static string GetUniqueKey(int size)
        {
            var bytes = new byte[size];
            using var crypto = RandomNumberGenerator.Create();
            crypto.GetBytes(bytes);
            var result = new StringBuilder(size);
            foreach (var b in bytes)
            {
                result.Append(Chars[b % Chars.Length]);
            }

            return result.ToString();
        }
    }
}