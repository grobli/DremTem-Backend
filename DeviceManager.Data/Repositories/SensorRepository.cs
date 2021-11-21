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

        public async Task<Sensor> GetByIdAsync(Guid userId, string deviceName, string sensorName)
        {
            return await DeviceManagerContext.Sensors
                .SingleOrDefaultAsync(s => s.UserId == userId && s.DeviceName == deviceName && s.Name == sensorName);
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync(Guid userId)
        {
            return await DeviceManagerContext.Sensors
                .Where(s => s.UserId == userId)
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
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<Sensor> GetWithSensorTypeByIdAsync(Guid userId, string deviceName, string sensorName)
        {
            return await DeviceManagerContext.Sensors
                .Include(s => s.Type)
                .SingleOrDefaultAsync(s => s.UserId == userId && s.DeviceName == deviceName && s.Name == sensorName);
        }
    }
}