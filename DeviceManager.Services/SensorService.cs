using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;

namespace DeviceManager.Services
{
    public class SensorService : ISensorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SensorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Sensor>> GetAllSensors()
        {
            return await _unitOfWork.Sensors.GetAllAsync();
        }

        public async Task<IEnumerable<Sensor>> GetAllSensorsWithType()
        {
            return await _unitOfWork.Sensors.GetAllWithSensorTypeAsync();
        }

        public async Task<IEnumerable<Sensor>> GetAllSensorsOfUser(Guid userId)
        {
            return await _unitOfWork.Sensors.GetAllAsync(userId);
        }

        public async Task<IEnumerable<Sensor>> GetAllSensorsOfUserWithType(Guid userId)
        {
            return await _unitOfWork.Sensors.GetAllWithSensorTypeAsync(userId);
        }

        public async Task<Sensor> GetSensor(Guid userId, string deviceName, string sensorName)
        {
            return await _unitOfWork.Sensors.GetByIdAsync(userId, deviceName, sensorName);
        }

        public async Task<Sensor> GetSensorWithType(Guid userId, string deviceName, string sensorName)
        {
            return await _unitOfWork.Sensors.GetWithSensorTypeByIdAsync(userId, deviceName, sensorName);
        }

        public async Task<Sensor> CreateSensor(Sensor newSensor)
        {
            await _unitOfWork.Sensors.AddAsync(newSensor);
            await _unitOfWork.CommitAsync();

            return newSensor;
        }

        public async Task UpdateSensor(Sensor sensorToBeUpdated, Sensor sensor)
        {
            sensorToBeUpdated.DisplayName = sensor.DisplayName;
            sensorToBeUpdated.TypeName = sensor.TypeName;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteSensor(Sensor sensor)
        {
            _unitOfWork.Sensors.Remove(sensor);
            await _unitOfWork.CommitAsync();
        }
    }
}