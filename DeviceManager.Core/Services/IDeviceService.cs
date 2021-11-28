using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface IDeviceService
    {
        IQueryable<Device> GetAllDevicesQuery(Guid userId = default);
        IQueryable<Device> GetDeviceQuery(int deviceId, Guid userId = default);

        Task<Device> CreateDeviceAsync(Device newDevice, CancellationToken cancellationToken = default);

        Task UpdateDeviceAsync(Device deviceToBeUpdated, Device device, CancellationToken cancellationToken = default);
        Task UpdateDeviceLastSeenAsync(Device device, CancellationToken cancellationToken = default);
        Task DeleteDeviceAsync(Device device, CancellationToken cancellationToken = default);
    }
}