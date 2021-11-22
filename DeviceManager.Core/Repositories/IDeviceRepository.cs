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

        Task<Device> GetByIdAsync(long deviceId);
        Task<Device> GetWithLocationByIdAsync(long deviceId);
        Task<Device> GetWithSensorsByIdAsync(long deviceId);
        Task<Device> GetWithEverythingByIdAsync(long deviceId);
    }
}