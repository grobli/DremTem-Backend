using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorRepository : IRepository<Sensor>
    {
        Task<IEnumerable<Sensor>> GetAllAsync(Guid? userId = null);
        Task<IEnumerable<Sensor>> GetAllWithSensorTypeAsync(Guid? userId = null);

        [return: MaybeNull]
        Task<Sensor> GetByIdAsync(int sensorId);

        [return: MaybeNull]
        Task<Sensor> GetWithSensorTypeByIdAsync(int sensorId);

        [return: MaybeNull]
        Task<Sensor> GetWithDeviceByIdAsync(int sensorId);
    }
}