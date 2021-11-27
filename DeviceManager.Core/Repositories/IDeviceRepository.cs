using System;
using System.Linq;
using DeviceManager.Core.Models;
using Shared.Repositories;

namespace DeviceManager.Core.Repositories
{
    public interface IDeviceRepository : IRepository<Device>
    {
        IQueryable<Device> GetDevices(Guid userId);
        IQueryable<Device> GetDeviceById(int deviceId, Guid userId = default);
    }
}