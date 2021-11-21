using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ISensorService
    {
        Task<IEnumerable<Sensor>> GetAllSensors();
        Task<IEnumerable<Sensor>> GetAllSensorsWithType();

        Task<IEnumerable<Sensor>> GetAllSensorsOfUser(Guid userId);
        Task<IEnumerable<Sensor>> GetAllSensorsOfUserWithType(Guid userId);

        Task<Sensor> GetSensor(Guid userId, string deviceName, string sensorName);
        Task<Sensor> GetSensorWithType(Guid userId, string deviceName, string sensorName);

        Task<Sensor> CreateSensor(Sensor newSensor);
        Task UpdateSensor(Sensor sensorToBeUpdated, Sensor sensor);
        Task DeleteSensor(Sensor sensor);
    }
}