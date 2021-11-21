using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllLocations();
        Task<IEnumerable<Location>> GetAllLocationsWithDevices();

        Task<IEnumerable<Location>> GetAllLocationsOfUser(Guid userId);
        Task<IEnumerable<Location>> GetAllLocationsOfUserWithDevices(Guid userId);

        Task<Location> GetLocation(Guid userId, string locationName);
        Task<Location> GetLocationWithDevices(Guid userId, string locationName);

        Task<Location> CreateLocation(Location newLocation);
        Task UpdateLocation(Location locationToBeUpdated, Location location);
        Task DeleteLocation(Location location);
    }
}