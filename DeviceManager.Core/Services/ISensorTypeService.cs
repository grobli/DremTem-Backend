using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ISensorTypeService
    {
        Task<IEnumerable<SensorType>> GetAllSensorTypes();

        Task<SensorType> GetSensorType(string typeName);

        Task<SensorType> CreateSensorType(SensorType newSensorType);
        Task UpdateSensorType(SensorType sensorTypeToBeUpdated, SensorType sensorType);
        Task DeleteSensorType(SensorType sensorType);
    }
}