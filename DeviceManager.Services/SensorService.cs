using System;
using System.Linq;
using System.Threading;
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

        public IQueryable<Sensor> GetAllSensorsQuery(Guid userId)
        {
            return _unitOfWork.Sensors.GetSensors(userId);
        }

        public IQueryable<Sensor> GetSensorQuery(int sensorId, Guid userId)
        {
            return _unitOfWork.Sensors.GetSensorById(sensorId, userId);
        }

        public async Task<Sensor> CreateSensorAsync(Sensor newSensor, CancellationToken cancellationToken = default)
        {
            newSensor.Created = DateTime.UtcNow;

            await _unitOfWork.Sensors.AddAsync(newSensor, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newSensor;
        }

        public async Task UpdateSensorAsync(Sensor sensorToBeUpdated, Sensor sensor,
            CancellationToken cancellationToken = default)
        {
            sensorToBeUpdated.LastModified = DateTime.UtcNow;

            sensorToBeUpdated.DisplayName = sensor.DisplayName;
            sensorToBeUpdated.TypeId = sensor.TypeId;

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteSensorAsync(Sensor sensor, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Sensors.Remove(sensor);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}