using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ISensorTypeService
    {
        IQueryable<SensorType> GetAllSensorTypesQuery();
        IQueryable<SensorType> GetSensorTypeQuery(int typeId);
        Task<SensorType> CreateSensorTypeAsync(SensorType newSensorType, CancellationToken cancellationToken = default);

        Task UpdateSensorTypeAsync(SensorType sensorTypeToBeUpdated, SensorType sensorType,
            CancellationToken cancellationToken = default);

        Task DeleteSensorTypeAsync(SensorType sensorType, CancellationToken cancellationToken = default);
    }
}