using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;
using DeviceManager.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Data.Repositories
{
    public class SensorRepository : Repository<Sensor>, ISensorRepository
    {
        private DeviceManagerContext DeviceManagerContext => Context as DeviceManagerContext;

        public SensorRepository(DeviceManagerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync(Guid? userId = null)
        {
            if (userId is null)
            {
                return await DeviceManagerContext.Sensors.ToListAsync();
            }

            return await DeviceManagerContext.Sensors
                .Include(s => s.Device)
                .Where(s => s.Device.UserId == userId)
                .ToListAsync();
        }


        public async Task<IEnumerable<Sensor>> GetAllWithSensorTypeAsync(Guid? userId = null)
        {
            var include = DeviceManagerContext.Sensors
                .Include(s => s.Type);

            if (userId is null)
            {
                return await include.ToListAsync();
            }

            return await include
                .Include(s => s.Device)
                .Where(s => s.Device.UserId == userId)
                .ToListAsync();
        }


        public async Task<Sensor> GetByIdAsync(long sensorId)
        {
            return await DeviceManagerContext.Sensors
                .SingleOrDefaultAsync(s => s.Id == sensorId);
        }


        public async Task<Sensor> GetWithSensorTypeByIdAsync(long sensorId)
        {
            return await DeviceManagerContext.Sensors
                .Include(s => s.Type)
                .SingleOrDefaultAsync(s => s.Id == sensorId);
        }
    }
}