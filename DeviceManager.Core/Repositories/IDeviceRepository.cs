using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<Device> GetByIdAsync(Guid userId, string deviceName);
        Task<IEnumerable<Device>> GetAllAsync(Guid userId);
        Task<IEnumerable<Device>> GetAllWithLocationAsync(Guid? userId = null);
        Task<Device> GetWithLocationByIdAsync(Guid userId, string deviceName);
        Task<IEnumerable<Device>> GetAllWithSensorsAsync(Guid? userId = null);
        Task<Device> GetWithSensorsByIdAsync(Guid userId, string deviceName);
        Task<IEnumerable<Device>> GetAllWithEverything(Guid? userId = null);
        Task<Device> GetWithEverythingByIdAsync(Guid userId, string deviceName);
    }
}