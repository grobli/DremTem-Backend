using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorRepository : IRepository<Sensor>
    {
        Task<IEnumerable<Sensor>> GetAllAsync(Guid? userId = null);
        Task<IEnumerable<Sensor>> GetAllWithSensorTypeAsync(Guid? userId = null);

        Task<Sensor> GetByIdAsync(int sensorId);
        Task<Sensor> GetWithSensorTypeByIdAsync(int sensorId);
        Task<Sensor> GetWithDeviceByIdAsync(int sensorId);
    }
}