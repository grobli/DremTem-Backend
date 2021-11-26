using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using DeviceManager.Data.Validators;
using FluentValidation;

namespace DeviceManager.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<Device> _validator;

        public DeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _validator = new DeviceValidator(_unitOfWork);
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

        public async Task<Device> GetDevice(int deviceId)
        {
            return await _unitOfWork.Devices.GetByIdAsync(deviceId);
        }

        public async Task<Device> GetDeviceWithLocation(int deviceId)
        {
            return await _unitOfWork.Devices.GetWithLocationByIdAsync(deviceId);
        }

        public async Task<Device> GetDeviceWithSensors(int deviceId)
        {
            return await _unitOfWork.Devices.GetWithSensorsByIdAsync(deviceId);
        }

        public async Task<Device> GetDeviceWithAll(int deviceId)
        {
            return await _unitOfWork.Devices.GetWithEverythingByIdAsync(deviceId);
        }

        /** <exception cref="ValidationException">device model is not valid</exception> */
        public async Task<Device> CreateDevice(Device newDevice, IEnumerable<Sensor> sensors)
        {
            await _validator.ValidateAndThrowAsync(newDevice);

            var now = DateTime.UtcNow;
            newDevice.Created = now;

            newDevice.MacAddress = newDevice.MacAddress;

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

        /** <exception cref="ValidationException">device model is not valid</exception> */
        public async Task UpdateDevice([NotNull] Device deviceToBeUpdated, [NotNull] Device device)
        {
            var backup = new Device
            {
                LastModified = deviceToBeUpdated.LastModified,
                DisplayName = deviceToBeUpdated.DisplayName,
                Online = deviceToBeUpdated.Online,
                Model = deviceToBeUpdated.Model,
                Manufacturer = deviceToBeUpdated.Manufacturer,
                LocationId = deviceToBeUpdated.LocationId
            };

            deviceToBeUpdated.LastModified = DateTime.UtcNow;
            deviceToBeUpdated.DisplayName = device.DisplayName;
            deviceToBeUpdated.Online = device.Online;
            deviceToBeUpdated.Model = device.Model;
            deviceToBeUpdated.Manufacturer = device.Manufacturer;
            deviceToBeUpdated.LocationId = device.LocationId;

            try
            {
                await _validator.ValidateAndThrowAsync(deviceToBeUpdated);
            }
            catch (ValidationException)
            {
                Restore();
                throw;
            }

            await _unitOfWork.CommitAsync();

            void Restore()
            {
                deviceToBeUpdated.LastModified = backup.LastModified;
                deviceToBeUpdated.DisplayName = backup.DisplayName;
                deviceToBeUpdated.Online = backup.Online;
                deviceToBeUpdated.Model = backup.Model;
                deviceToBeUpdated.Manufacturer = backup.Manufacturer;
                deviceToBeUpdated.LocationId = backup.LocationId;
            }
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