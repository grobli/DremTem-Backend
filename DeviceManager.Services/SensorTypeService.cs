using System;
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
    public class SensorTypeService : ISensorTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IValidator<SensorType> _validator;
        private IValidator<SensorType> Validator => _validator ??= new SensorTypeValidator(_unitOfWork);

        public SensorTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<SensorType> GetAllSensorTypesQuery()
        {
            return _unitOfWork.SensorTypes.FindAll();
        }

        public IQueryable<SensorType> GetSensorTypeQuery(int typeId)
        {
            return _unitOfWork.SensorTypes.GetSensorTypeById(typeId);
        }

        public async Task<SensorType> CreateSensorTypeAsync(SensorType newSensorType,
            CancellationToken cancellationToken = default)
        {
            newSensorType.Created = DateTime.UtcNow;
            await Validator.ValidateAndThrowAsync(newSensorType, cancellationToken);

            await _unitOfWork.SensorTypes.AddAsync(newSensorType, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newSensorType;
        }

        public async Task UpdateSensorTypeAsync(SensorType sensorTypeToBeUpdated, SensorType sensorType,
            CancellationToken cancellationToken = default)
        {
            var backup = sensorTypeToBeUpdated with { };

            sensorTypeToBeUpdated.LastModified = DateTime.UtcNow;
            sensorTypeToBeUpdated.Unit = sensorType.Unit;
            sensorTypeToBeUpdated.UnitShort = sensorType.UnitShort;
            sensorTypeToBeUpdated.UnitSymbol = sensorType.UnitSymbol;
            sensorTypeToBeUpdated.IsDiscrete = sensorType.IsDiscrete;
            sensorTypeToBeUpdated.IsSummable = sensorType.IsSummable;

            var validationResult = await Validator.ValidateAsync(sensorTypeToBeUpdated, cancellationToken);
            if (!validationResult.IsValid)
            {
                Restore();
                throw new ValidationException(validationResult.Errors);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            void Restore()
            {
                sensorTypeToBeUpdated.LastModified = backup.LastModified;
                sensorTypeToBeUpdated.Unit = backup.Unit;
                sensorTypeToBeUpdated.UnitShort = backup.UnitShort;
                sensorTypeToBeUpdated.UnitSymbol = backup.UnitSymbol;
                sensorTypeToBeUpdated.IsDiscrete = backup.IsDiscrete;
                sensorTypeToBeUpdated.IsSummable = backup.IsSummable;
            }
        }

        public async Task DeleteSensorTypeAsync(SensorType sensorType, CancellationToken cancellationToken = default)
        {
            _unitOfWork.SensorTypes.Remove(sensorType);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}