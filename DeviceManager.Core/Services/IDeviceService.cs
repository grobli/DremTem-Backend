using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetAllDevices(Guid? userId = null);
        Task<IEnumerable<Device>> GetAllDevicesWithLocation(Guid? userId = null);
        Task<IEnumerable<Device>> GetAllDevicesWithSensors(Guid? userId = null);
        Task<IEnumerable<Device>> GetAllDevicesWithAll(Guid? userId = null);

        Task<Device> GetDevice(Guid userId, string deviceName);
        Task<Device> GetDeviceWithLocation(Guid userId, string deviceName);
        Task<Device> GetDeviceWithSensors(Guid userId, string deviceName);
        Task<Device> GetDeviceWithAll(Guid userId, string deviceName);


        Task<Device> CreateDevice(Device newDevice, IEnumerable<Sensor> sensors);
        Task UpdateDevice(Device deviceToBeUpdated, Device device);
        Task UpdateDeviceLastSeen(Device device);
        Task DeleteDevice(Device device);
    }
}