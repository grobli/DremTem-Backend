using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services.DeviceTokenService;
using Microsoft.Extensions.Options;

namespace DeviceManager.Services
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly TokenConfig _config;
        private const int SecurityStampLength = 16;

        public DeviceTokenService(IOptions<TokenConfig> config)
        {
            _config = config.Value;
        }

        public string GenerateToken(Device device)
        {
            var tokenContent = new TokenContent
            {
                DeviceId = device.Id,
                SecurityStamp = KeyGenerator.GetUniqueKey(SecurityStampLength)
            };

            var tokenJson = JsonSerializer.Serialize(tokenContent);

            return EncryptString(tokenJson);
        }

        public async Task<string> GenerateTokenAsync(Device device)
        {
            return await Task.Run(() => GenerateToken(device));
        }

        public TokenContent DecodeToken(string token)
        {
            var tokenJson = DecryptString(token);
            try
            {
                return JsonSerializer.Deserialize<TokenContent>(tokenJson);
            }
            catch (Exception)
            {
                throw new DecodeTokenException(tokenJson);
            }
        }

        public async Task<TokenContent> DecodeTokenAsync(string token)
        {
            return await Task.Run(() => DecodeToken(token));
        }

        private string EncryptString(string plainText)
        {
            var iv = new byte[16];

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_config.SecretKey);
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var cipherText = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            return Convert.ToBase64String(cipherText);
        }

        private string DecryptString(string cipherText)
        {
            var iv = new byte[16];

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_config.SecretKey);
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var bytes = Convert.FromBase64String(cipherText);
            var plainBytes = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }

    public class DecodeTokenException : Exception
    {
        public DecodeTokenException()
        {
        }

        public DecodeTokenException(string tokenContent) : base(
            $"Failed to decode token. Token has invalid content: {tokenContent}")
        {
        }
    }
}