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


        public async Task<IEnumerable<Sensor>> GetAllSensors(Guid? userId = null)
        {
            return await _unitOfWork.Sensors.GetAllAsync(userId);
        }

        public async Task<IEnumerable<Sensor>> GetAllSensorsWithType(Guid? userId = null)
        {
            return await _unitOfWork.Sensors.GetAllWithSensorTypeAsync(userId);
        }

        public async Task<Sensor> GetSensor(int sensorId)
        {
            return await _unitOfWork.Sensors.GetByIdAsync(sensorId);
        }

        public async Task<Sensor> GetSensorWithType(int sensorId)
        {
            return await _unitOfWork.Sensors.GetWithSensorTypeByIdAsync(sensorId);
        }

        public async Task<Sensor> GetSensorWithDevice(int sensorId)
        {
            return await _unitOfWork.Sensors.GetWithDeviceByIdAsync(sensorId);
        }

        public async Task<Sensor> CreateSensor(Sensor newSensor)
        {
            newSensor.Created = DateTime.UtcNow;

            await _unitOfWork.Sensors.AddAsync(newSensor);
            await _unitOfWork.CommitAsync();

            return newSensor;
        }

        public async Task UpdateSensor(Sensor sensorToBeUpdated, Sensor sensor)
        {
            sensorToBeUpdated.LastModified = DateTime.UtcNow;

            sensorToBeUpdated.DisplayName = sensor.DisplayName;
            sensorToBeUpdated.TypeId = sensor.TypeId;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteSensor(Sensor sensor)
        {
            _unitOfWork.Sensors.Remove(sensor);
            await _unitOfWork.CommitAsync();
        }
    }
}