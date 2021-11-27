using System;
using System.Linq;
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

        public IQueryable<SensorType> GetAllSensorTypes()
        {
            return _unitOfWork.SensorTypes.FindAll();
        }

        public IQueryable<SensorType> GetSensorType(int typeId)
        {
            return _unitOfWork.SensorTypes.GetSensorTypeById(typeId);
        }

        public async Task<SensorType> CreateSensorTypeAsync(SensorType newSensorType)
        {
            newSensorType.Created = DateTime.UtcNow;

            await _unitOfWork.SensorTypes.AddAsync(newSensorType);
            await _unitOfWork.CommitAsync();

            return newSensorType;
        }

        public async Task UpdateSensorTypeAsync(SensorType sensorTypeToBeUpdated, SensorType sensorType)
        {
            sensorTypeToBeUpdated.LastModified = DateTime.UtcNow;

            sensorTypeToBeUpdated.Unit = sensorType.Unit;
            sensorTypeToBeUpdated.UnitShort = sensorType.UnitShort;
            sensorTypeToBeUpdated.UnitSymbol = sensorType.UnitSymbol;
            sensorTypeToBeUpdated.DataType = sensorType.DataType;
            sensorTypeToBeUpdated.IsDiscrete = sensorType.IsDiscrete;
            sensorTypeToBeUpdated.IsSummable = sensorType.IsSummable;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteSensorTypeAsync(SensorType sensorType)
        {
            _unitOfWork.SensorTypes.Remove(sensorType);
            await _unitOfWork.CommitAsync();
        }
    }
}