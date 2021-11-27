using System;
using System.Linq;
using DeviceManager.Core.Models;
using Shared.Repositories;

namespace DeviceManager.Core.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        IQueryable<Location> GetLocations(Guid userId = default);
        IQueryable<Location> GetLocationById(int locationId, Guid userId = default);
    }
}