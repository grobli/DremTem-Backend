using System;
using System.Linq;
using DeviceManager.Core.Models;
using Shared.Repositories;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorRepository : IRepository<Sensor>
    {
        IQueryable<Sensor> GetSensors(Guid userId = default);
        IQueryable<Sensor> GetSensorById(int sensorId, Guid userId = default);
    }
}