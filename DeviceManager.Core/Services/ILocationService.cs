using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllLocations(Guid? userId = null);
        Task<IEnumerable<Location>> GetAllLocationsWithDevices(Guid? userId = null);

        Task<Location> GetLocation(int locationId);
        Task<Location> GetLocationWithDevices(int locationId);

        Task<Location> CreateLocation(Location newLocation);
        Task UpdateLocation(Location locationToBeUpdated, Location location);
        Task DeleteLocation(Location location);
    }
}