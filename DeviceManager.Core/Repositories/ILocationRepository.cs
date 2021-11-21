using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<Location> GetByIdAsync(Guid userId, string locationName);
        Task<IEnumerable<Location>> GetAllAsync(Guid userId);

        Task<IEnumerable<Location>> GetAllWithDevicesAsync(Guid? userId = null);
        Task<Location> GetWithDevicesByIdAsync(Guid userId, string locationName);
    }
}