using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;

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
            return await _unitOfWork.Devices.GetAllAsync(userId);
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

        public async Task<Device> GetDevice(long deviceId)
        {
            return await _unitOfWork.Devices.GetByIdAsync(deviceId);
        }

        public async Task<Device> GetDeviceWithLocation(long deviceId)
        {
            return await _unitOfWork.Devices.GetWithLocationByIdAsync(deviceId);
        }

        public async Task<Device> GetDeviceWithSensors(long deviceId)
        {
            return await _unitOfWork.Devices.GetWithSensorsByIdAsync(deviceId);
        }

        public async Task<Device> GetDeviceWithAll(long deviceId)
        {
            return await _unitOfWork.Devices.GetWithEverythingByIdAsync(deviceId);
        }

        public async Task<Device> CreateDevice(Device newDevice, IEnumerable<Sensor> sensors)
        {
            var now = DateTime.UtcNow;
            newDevice.Created = now;

            await _unitOfWork.Devices.AddAsync(newDevice);
            await _unitOfWork.CommitAsync();

            await _unitOfWork.Sensors.AddRangeAsync(sensors.Select(s =>
            {
                s.DeviceId = newDevice.Id;
                s.Created = now;
                return s;
            }));
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