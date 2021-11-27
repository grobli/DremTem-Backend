using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ILocationService
    {
        IQueryable<Location> GetAllLocations(Guid userId = default);
        IQueryable<Location> GetLocation(int locationId, Guid userId = default);

        Task<Location> CreateLocationAsync(Location newLocation);
        Task UpdateLocationAsync(Location locationToBeUpdated, Location location);
        Task DeleteLocationAsync(Location location);
    }
}