using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorRepository : IRepository<Sensor>
    {
        Task<Sensor> GetByIdAsync(Guid userId, string deviceName, string sensorName);
        Task<IEnumerable<Sensor>> GetAllAsync(Guid userId);

        Task<IEnumerable<Sensor>> GetAllWithSensorTypeAsync(Guid? userId = null);
        Task<Sensor> GetWithSensorTypeByIdAsync(Guid userId, string deviceName, string sensorName);
    }
}