using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface IDeviceTokenService
    {
        string GenerateToken(Device device);
        Task<string> GenerateTokenAsync(Device device);
    }
}