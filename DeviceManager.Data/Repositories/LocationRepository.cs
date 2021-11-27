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
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        private DeviceManagerContext DeviceManagerContext => Context as DeviceManagerContext;

        public LocationRepository(DeviceManagerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Location>> GetAllAsync(Guid? userId = null)
        {
            if (userId is null)
            {
                return await DeviceManagerContext.Locations.ToListAsync();
            }

            return await DeviceManagerContext.Locations
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }


        public async Task<IEnumerable<Location>> GetAllWithDevicesAsync(Guid? userId = null)
        {
            var include = DeviceManagerContext.Locations
                .Include(l => l.Devices);

            if (userId is null)
            {
                return await include.ToListAsync();
            }

            return await include
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<Location> GetByIdAsync(int locationId)
        {
            return await DeviceManagerContext.Locations
                .SingleOrDefaultAsync(l => l.Id == locationId);
        }

        public async Task<Location> GetWithDevicesByIdAsync(int locationId)
        {
            return await DeviceManagerContext.Locations
                .Include(l => l.Devices)
                .SingleOrDefaultAsync(l => l.Id == locationId);
        }

        public IQueryable<Location> GetLocations(Guid userId)
        {
            var locations = userId == Guid.Empty
                ? DeviceManagerContext.Locations
                : DeviceManagerContext.Locations.Where(l => l.UserId == userId);

            return locations.OrderBy(l => l.Name);
        }

        public IQueryable<Location> GetLocationById(int locationId, Guid userId)
        {
            var locations = userId == Guid.Empty
                ? DeviceManagerContext.Locations
                : DeviceManagerContext.Locations.Where(d => d.UserId == userId);

            return locations.Where(l => l.Id == locationId).Take(1);
        }
    }
}