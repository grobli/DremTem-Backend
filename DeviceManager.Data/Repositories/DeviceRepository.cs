using System;
using System.Linq;
using DeviceManager.Core.Models;
using DeviceManager.Core.Repositories;
using Shared.Repositories;

namespace DeviceManager.Data.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        private DeviceManagerContext DeviceManagerContext => Context as DeviceManagerContext;

        public DeviceRepository(DeviceManagerContext context) : base(context)
        {
        }

        public IQueryable<Device> GetDevices(Guid userId)
        {
            var devices = userId == Guid.Empty
                ? DeviceManagerContext.Devices
                : DeviceManagerContext.Devices.Where(d => d.UserId == userId);

            return devices.OrderBy(d => d.Name);
        }

        public IQueryable<Device> GetDeviceById(int deviceId, Guid userId = default)
        {
            var devices = userId == Guid.Empty
                ? DeviceManagerContext.Devices
                : DeviceManagerContext.Devices.Where(d => d.UserId == userId);

            return devices.Where(d => d.Id == deviceId).Take(1);
        }
    }
}