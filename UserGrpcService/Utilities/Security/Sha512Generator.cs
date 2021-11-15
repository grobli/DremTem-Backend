using System;
using System.Security.Cryptography;
using System.Text;

namespace UserGrpcService.Utilities.Security
{
    public static class Sha512Generator
    {
        public static string CreateHash(string input)
        {
            using var sha512 = new SHA512Managed();
            var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}