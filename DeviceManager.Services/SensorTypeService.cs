using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;

namespace DeviceManager.Services
{
    public class SensorTypeService : ISensorTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

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

            await _unitOfWork.SensorTypes.AddAsync(newSensorType, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newSensorType;
        }

        public async Task UpdateSensorTypeAsync(SensorType sensorTypeToBeUpdated, SensorType sensorType,
            CancellationToken cancellationToken = default)
        {
            sensorTypeToBeUpdated.LastModified = DateTime.UtcNow;

            sensorTypeToBeUpdated.Unit = sensorType.Unit;
            sensorTypeToBeUpdated.UnitShort = sensorType.UnitShort;
            sensorTypeToBeUpdated.UnitSymbol = sensorType.UnitSymbol;
            sensorTypeToBeUpdated.DataType = sensorType.DataType;
            sensorTypeToBeUpdated.IsDiscrete = sensorType.IsDiscrete;
            sensorTypeToBeUpdated.IsSummable = sensorType.IsSummable;

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteSensorTypeAsync(SensorType sensorType, CancellationToken cancellationToken = default)
        {
            _unitOfWork.SensorTypes.Remove(sensorType);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}