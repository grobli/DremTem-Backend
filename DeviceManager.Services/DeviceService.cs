﻿using System;
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
            await Validator.ValidateAndThrowAsync(newDevice, cancellationToken);
            await _unitOfWork.Devices.AddAsync(newDevice, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newDevice;
        }

        /** <exception cref="ValidationException">device model is not valid</exception> */
        public async Task UpdateDeviceAsync([NotNull] Device deviceToBeUpdated, [NotNull] Device device,
            CancellationToken cancellationToken = default)
        {
            var backup = new Device(deviceToBeUpdated);

            device.LastModified = DateTime.UtcNow;
            deviceToBeUpdated.MapEditableFields(device);

            var validationResult = await Validator.ValidateAsync(deviceToBeUpdated, cancellationToken);
            if (!validationResult.IsValid)
            {
                deviceToBeUpdated.MapEditableFields(backup);
                throw new ValidationException(validationResult.Errors);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
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