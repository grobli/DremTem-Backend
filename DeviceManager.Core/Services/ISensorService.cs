using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ISensorService
    {
        IQueryable<Sensor> GetAllSensors(Guid userId);
        IQueryable<Sensor> GetSensor(int sensorId, Guid userId);
        Task<Sensor> CreateSensorAsync(Sensor newSensor);
        Task UpdateSensorAsync(Sensor sensorToBeUpdated, Sensor sensor);
        Task DeleteSensorAsync(Sensor sensor);
    }
}