using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services.DeviceTokenService
{
    public interface IDeviceTokenService
    {
        string GenerateToken(Device device);
        Task<string> GenerateTokenAsync(Device device);

        TokenContent DecodeToken(string token);
        Task<TokenContent> DecodeTokenAsync(string token);
    }
}