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

        Task<Location> GetByIdAsync(int locationId);
        Task<Location> GetWithDevicesByIdAsync(int locationId);
    }
}