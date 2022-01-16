using System;
using System.Collections.Generic;
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
    public class SensorService : ISensorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IValidator<Sensor> _validator;
        private IValidator<Sensor> Validator => _validator ??= new SensorValidator(_unitOfWork);

        public SensorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Sensor> GetAllSensorsQuery(Guid userId)
        {
            return _unitOfWork.Sensors.GetSensors(userId);
        }

        public IQueryable<Sensor> GetSensorQuery(int sensorId, Guid userId)
        {
            return _unitOfWork.Sensors.GetSensorById(sensorId, userId);
        }

        public async Task<Sensor> CreateSensorAsync(Sensor newSensor, CancellationToken cancellationToken = default)
        {
            await Validator.ValidateAndThrowAsync(newSensor, cancellationToken);
            await _unitOfWork.Sensors.AddAsync(newSensor, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newSensor;
        }

        public async Task<IEnumerable<Sensor>> CreateSensorsRangeAsync(IEnumerable<Sensor> newSensors,
            CancellationToken cancellationToken = default)
        {
            var sensors = newSensors as Sensor[] ?? newSensors.ToArray();
            foreach (var sensor in sensors)
            {
                await Validator.ValidateAndThrowAsync(sensor, cancellationToken);
            }

            await _unitOfWork.Sensors.AddRangeAsync(sensors, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return sensors;
        }

        public async Task UpdateSensorAsync(Sensor sensorToBeUpdated, Sensor sensor,
            CancellationToken cancellationToken = default)
        {
            var backup = new Sensor(sensorToBeUpdated);

            sensor.LastModified = DateTime.UtcNow;
            sensorToBeUpdated.MapEditableFields(sensor);

            var validationResult = await Validator.ValidateAsync(sensorToBeUpdated, cancellationToken);
            if (!validationResult.IsValid)
            {
                sensorToBeUpdated.MapEditableFields(backup);
                throw new ValidationException(validationResult.Errors);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteSensorAsync(Sensor sensor, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Sensors.Remove(sensor);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}