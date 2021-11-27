using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface IDeviceService
    {
        IQueryable<Device> GetAllDevices(Guid userId = default);
        IQueryable<Device> GetDevice(int deviceId, Guid userId = default);

        Task<Device> CreateDeviceAsync(Device newDevice, IEnumerable<Sensor> sensors);
        Task UpdateDeviceAsync(Device deviceToBeUpdated, Device device);
        Task UpdateDeviceLastSeenAsync(Device device);
        Task DeleteDeviceAsync(Device device);
    }
}