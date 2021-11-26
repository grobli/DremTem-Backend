using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;
using DeviceManager.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;

namespace DeviceManager.Data.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        private DeviceManagerContext DeviceManagerContext => Context as DeviceManagerContext;


        public DeviceRepository(DeviceManagerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Device>> GetAllAsync(Guid? userId = null)
        {
            if (userId is null)
            {
                return await DeviceManagerContext.Devices.ToListAsync();
            }
            
            return await DeviceManagerContext.Devices
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Device>> GetAllWithLocationAsync(Guid? userId = null)
        {
            var include = DeviceManagerContext.Devices
                .Include(d => d.Location);

            if (userId is null)
            {
                return await include.ToListAsync();
            }

            return await include
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Device>> GetAllWithSensorsAsync(Guid? userId = null)
        {
            var include = DeviceManagerContext.Devices
                .Include(d => d.Sensors);

            if (userId is null)
            {
                return await include.ToListAsync();
            }

            return await include
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Device>> GetAllWithEverything(Guid? userId = null)
        {
            var include = DeviceManagerContext.Devices
                .Include(d => d.Location)
                .Include(d => d.Sensors);

            if (userId is null)
            {
                return await include.ToListAsync();
            }

            return await include
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }


        public async Task<Device> GetByIdAsync(int deviceId)
        {
            return await DeviceManagerContext.Devices
                .SingleOrDefaultAsync(d => d.Id == deviceId);
        }


        public async Task<Device> GetWithLocationByIdAsync(int deviceId)
        {
            return await DeviceManagerContext.Devices
                .Include(d => d.Location)
                .SingleOrDefaultAsync(d => d.Id == deviceId);
        }


        public async Task<Device> GetWithSensorsByIdAsync(int deviceId)
        {
            return await DeviceManagerContext.Devices
                .Include(d => d.Sensors)
                .SingleOrDefaultAsync(d => d.Id == deviceId);
        }


        public async Task<Device> GetWithEverythingByIdAsync(int deviceId)
        {
            return await DeviceManagerContext.Devices
                .Include(d => d.Location)
                .Include(d => d.Sensors)
                .SingleOrDefaultAsync(d => d.Id == deviceId);
        }
    }
}