using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using DeviceManager.Data.Validation;
using FluentValidation;

namespace DeviceManager.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IValidator<Device> _validator;
        private IValidator<Device> Validator => _validator ??= new DeviceValidator(_unitOfWork);

        public DeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Device> GetAllDevicesQuery(Guid userId = default)
        {
            return _unitOfWork.Devices.GetDevices(userId);
        }

        public IQueryable<Device> GetDeviceQuery(int deviceId, Guid userId = default)
        {
            return _unitOfWork.Devices.GetDeviceById(deviceId, userId);
        }

        /** <exception cref="ValidationException">device model is not valid</exception> */
        public async Task<Device> CreateDeviceAsync(Device newDevice, CancellationToken cancellationToken = default)
        {
            newDevice.Created = DateTime.UtcNow;

            await Validator.ValidateAndThrowAsync(newDevice, cancellationToken);

            await _unitOfWork.Devices.AddAsync(newDevice, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newDevice;
        }

        /** <exception cref="ValidationException">device model is not valid</exception> */
        public async Task UpdateDeviceAsync([NotNull] Device deviceToBeUpdated, [NotNull] Device device,
            CancellationToken cancellationToken = default)
        {
            var backup = deviceToBeUpdated with { };

            deviceToBeUpdated.LastModified = DateTime.UtcNow;
            deviceToBeUpdated.DisplayName = device.DisplayName;
            deviceToBeUpdated.Online = device.Online;
            deviceToBeUpdated.Model = device.Model;
            deviceToBeUpdated.Manufacturer = device.Manufacturer;
            deviceToBeUpdated.LocationId = device.LocationId;
            deviceToBeUpdated.MacAddress = device.MacAddress;

            var validationResult = await Validator.ValidateAsync(deviceToBeUpdated, cancellationToken);
            if (!validationResult.IsValid)
            {
                Restore();
                throw new ValidationException(validationResult.Errors);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            void Restore()
            {
                deviceToBeUpdated.LastModified = backup.LastModified;
                deviceToBeUpdated.DisplayName = backup.DisplayName;
                deviceToBeUpdated.Online = backup.Online;
                deviceToBeUpdated.Model = backup.Model;
                deviceToBeUpdated.Manufacturer = backup.Manufacturer;
                deviceToBeUpdated.LocationId = backup.LocationId;
                deviceToBeUpdated.MacAddress = backup.MacAddress;
            }
        }

        public async Task UpdateDeviceLastSeenAsync(Device device,
            CancellationToken cancellationToken = default)
        {
            device.LastSeen = DateTime.UtcNow;
            await _unitOfWork.CommitAsync(cancellationToken);
        }


        public async Task DeleteDeviceAsync(Device device, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Devices.Remove(device);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}