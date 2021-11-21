using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using DeviceManager.Data.Configurations;

namespace DeviceManager.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Device>> GetAllDevices(Guid? userId = null)
        {
            return userId is null
                ? await _unitOfWork.Devices.GetAllAsync()
                : await _unitOfWork.Devices.GetAllAsync(userId.Value);
        }

        public async Task<IEnumerable<Device>> GetAllDevicesWithLocation(Guid? userId = null)
        {
            return await _unitOfWork.Devices.GetAllWithLocationAsync(userId);
        }

        public async Task<IEnumerable<Device>> GetAllDevicesWithSensors(Guid? userId = null)
        {
            return await _unitOfWork.Devices.GetAllWithSensorsAsync(userId);
        }

        public async Task<IEnumerable<Device>> GetAllDevicesWithAll(Guid? userId = null)
        {
            return await _unitOfWork.Devices.GetAllWithEverything(userId);
        }

        public async Task<Device> GetDevice(Guid userId, string deviceName)
        {
            return await _unitOfWork.Devices.GetByIdAsync(userId, deviceName);
        }

        public async Task<Device> GetDeviceWithLocation(Guid userId, string deviceName)
        {
            return await _unitOfWork.Devices.GetWithLocationByIdAsync(userId, deviceName);
        }

        public async Task<Device> GetDeviceWithSensors(Guid userId, string deviceName)
        {
            return await _unitOfWork.Devices.GetWithSensorsByIdAsync(userId, deviceName);
        }

        public async Task<Device> GetDeviceWithAll(Guid userId, string deviceName)
        {
            return await _unitOfWork.Devices.GetWithEverythingByIdAsync(userId, deviceName);
        }

        public async Task<Device> CreateDevice(Device newDevice, IEnumerable<Sensor> sensors)
        {
            newDevice.Created = DateTime.UtcNow;
            newDevice.ApiKey = KeyGenerator.GetUniqueKey(DeviceConfiguration.ApiKeyMaxLength);

            await _unitOfWork.Devices.AddAsync(newDevice);
            await _unitOfWork.Sensors.AddRangeAsync(sensors);
            
            await _unitOfWork.CommitAsync();

            return newDevice;
        }

        public async Task UpdateDevice(Device deviceToBeUpdated, Device device)
        {
            deviceToBeUpdated.LastModified = DateTime.UtcNow;
            deviceToBeUpdated.LocationName = device.LocationName;
            deviceToBeUpdated.Online = device.Online;
            deviceToBeUpdated.DisplayName = device.DisplayName;

            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateDeviceLastSeen(Device device)
        {
            device.LastSeen = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
        }


        public async Task DeleteDevice(Device device)
        {
            _unitOfWork.Devices.Remove(device);
            await _unitOfWork.CommitAsync();
        }
    }
}