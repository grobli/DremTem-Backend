using System;
using System.Linq;
using DeviceManager.Core.Models;
using DeviceManager.Core.Repositories;
using Shared.Repositories;

namespace DeviceManager.Data.Repositories
{
    public class SensorRepository : Repository<Sensor>, ISensorRepository
    {
        private DeviceManagerContext DeviceManagerContext => Context as DeviceManagerContext;

        public SensorRepository(DeviceManagerContext context) : base(context)
        {
        }

        public IQueryable<Sensor> GetSensors(Guid userId)
        {
            var sensors = userId == Guid.Empty
                ? DeviceManagerContext.Sensors
                : DeviceManagerContext.Sensors.Where(s => s.Device.UserId == userId);

            return sensors.OrderBy(d => d.Name);
        }

        public IQueryable<Sensor> GetSensorById(int sensorId, Guid userId)
        {
            var sensors = userId == Guid.Empty
                ? DeviceManagerContext.Sensors
                : DeviceManagerContext.Sensors.Where(s => s.Device.UserId == userId);

            return sensors.Where(d => d.Id == sensorId).Take(1);
        }
    }
}