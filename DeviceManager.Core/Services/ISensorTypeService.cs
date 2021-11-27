using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ISensorTypeService
    {
        IQueryable<SensorType> GetAllSensorTypes();
        IQueryable<SensorType> GetSensorType(int typeId);
        Task<SensorType> CreateSensorTypeAsync(SensorType newSensorType);
        Task UpdateSensorTypeAsync(SensorType sensorTypeToBeUpdated, SensorType sensorType);
        Task DeleteSensorTypeAsync(SensorType sensorType);
    }
}