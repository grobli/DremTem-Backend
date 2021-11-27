using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ISensorService
    {
        IQueryable<Sensor> GetAllSensorsQuery(Guid userId);
        IQueryable<Sensor> GetSensorQuery(int sensorId, Guid userId);
        Task<Sensor> CreateSensorAsync(Sensor newSensor, CancellationToken cancellationToken = default);
        Task UpdateSensorAsync(Sensor sensorToBeUpdated, Sensor sensor, CancellationToken cancellationToken = default);
        Task DeleteSensorAsync(Sensor sensor, CancellationToken cancellationToken = default);
    }
}