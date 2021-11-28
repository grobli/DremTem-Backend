using System;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Core
{
    public interface IDeviceManagerContext : IDisposable
    {
        DbSet<Device> Devices { get; set; }
        DbSet<Location> Locations { get; set; }
        DbSet<Sensor> Sensors { get; set; }
        DbSet<SensorType> SensorTypes { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}