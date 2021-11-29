using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configs;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace DeviceManager.Services
{
    public class DeviceTokenService : IDeviceTokenService

    {
        private readonly JwtConfig _config;

        public DeviceTokenService(IOptions<JwtConfig> jwtSettings)
        {
            _config = jwtSettings.Value;
        }

        public string GenerateToken(Device device)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, device.Id.ToString()),
                new(ClaimTypes.NameIdentifier, device.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_config.ExpirationInDays));

            var token = new JwtSecurityToken(
                _config.Issuer,
                _config.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateTokenAsync(Device device, CancellationToken cancellationToken)
        {
            return await Task.Run(() => GenerateToken(device), cancellationToken);
        }
    }
}