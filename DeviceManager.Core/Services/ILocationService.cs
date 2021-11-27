using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface ILocationService
    {
        IQueryable<Location> GetAllLocationsQuery(Guid userId = default);
        IQueryable<Location> GetLocationQuery(int locationId, Guid userId = default);

        Task<Location> CreateLocationAsync(Location newLocation, CancellationToken cancellationToken = default);

        Task UpdateLocationAsync(Location locationToBeUpdated, Location location,
            CancellationToken cancellationToken = default);

        Task DeleteLocationAsync(Location location, CancellationToken cancellationToken = default);
    }
}