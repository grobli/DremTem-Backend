using System;
using System.Linq;
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

        public IQueryable<Sensor> GetAllSensors(Guid userId)
        {
            return _unitOfWork.Sensors.GetSensors(userId);
        }

        public IQueryable<Sensor> GetSensor(int sensorId, Guid userId)
        {
            return _unitOfWork.Sensors.GetSensorById(sensorId, userId);
        }

        public async Task<Sensor> CreateSensorAsync(Sensor newSensor)
        {
            newSensor.Created = DateTime.UtcNow;

            await _unitOfWork.Sensors.AddAsync(newSensor);
            await _unitOfWork.CommitAsync();

            return newSensor;
        }

        public async Task UpdateSensorAsync(Sensor sensorToBeUpdated, Sensor sensor)
        {
            sensorToBeUpdated.LastModified = DateTime.UtcNow;

            sensorToBeUpdated.DisplayName = sensor.DisplayName;
            sensorToBeUpdated.TypeId = sensor.TypeId;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteSensorAsync(Sensor sensor)
        {
            _unitOfWork.Sensors.Remove(sensor);
            await _unitOfWork.CommitAsync();
        }
    }
}