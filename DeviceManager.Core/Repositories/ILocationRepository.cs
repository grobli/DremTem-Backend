using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<IEnumerable<Location>> GetAllAsync(Guid? userId = null);
        Task<IEnumerable<Location>> GetAllWithDevicesAsync(Guid? userId = null);

        Task<Location> GetByIdAsync(Guid userId, string locationName);
        Task<Location> GetWithDevicesByIdAsync(Guid userId, string locationName);
    }
}