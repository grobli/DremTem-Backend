using System.Collections.Generic;
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

        public async Task<IEnumerable<SensorType>> GetAllSensorTypes()
        {
            return await _unitOfWork.SensorTypes.GetAllAsync();
        }

        public async Task<SensorType> GetSensorType(string typeName)
        {
            return await _unitOfWork.SensorTypes.GetByIdAsync(typeName);
        }

        public async Task<SensorType> CreateSensorType(SensorType newSensorType)
        {
            await _unitOfWork.SensorTypes.AddAsync(newSensorType);
            await _unitOfWork.CommitAsync();

            return newSensorType;
        }

        public async Task UpdateSensorType(SensorType sensorTypeToBeUpdated, SensorType sensorType)
        {
            sensorTypeToBeUpdated.Unit = sensorType.Unit;
            sensorTypeToBeUpdated.UnitShort = sensorType.UnitShort;
            sensorTypeToBeUpdated.UnitSymbol = sensorType.UnitSymbol;
            sensorTypeToBeUpdated.DataType = sensorType.DataType;
            sensorTypeToBeUpdated.IsDiscrete = sensorType.IsDiscrete;
            sensorTypeToBeUpdated.IsSummable = sensorType.IsSummable;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteSensorType(SensorType sensorType)
        {
            _unitOfWork.SensorTypes.Remove(sensorType);
            await _unitOfWork.CommitAsync();
        }
    }
}