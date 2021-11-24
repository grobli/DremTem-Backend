using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<IEnumerable<Device>> GetAllAsync(Guid? userId = null);
        Task<IEnumerable<Device>> GetAllWithLocationAsync(Guid? userId = null);
        Task<IEnumerable<Device>> GetAllWithSensorsAsync(Guid? userId = null);
        Task<IEnumerable<Device>> GetAllWithEverything(Guid? userId = null);

        Task<Device> GetByIdAsync(int deviceId);
        Task<Device> GetWithLocationByIdAsync(int deviceId);
        Task<Device> GetWithSensorsByIdAsync(int deviceId);
        Task<Device> GetWithEverythingByIdAsync(int deviceId);
    }
}