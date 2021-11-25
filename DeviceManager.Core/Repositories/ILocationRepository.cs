using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<IEnumerable<Location>> GetAllAsync(Guid? userId = null);
        Task<IEnumerable<Location>> GetAllWithDevicesAsync(Guid? userId = null);

        [return: MaybeNull]
        Task<Location> GetByIdAsync(int locationId);

        [return: MaybeNull]
        Task<Location> GetWithDevicesByIdAsync(int locationId);
    }
}