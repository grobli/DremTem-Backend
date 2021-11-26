using DeviceManager.Core.Models;
using DeviceManager.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace DeviceManager.Data
{
    public class DeviceManagerContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorType> SensorTypes { get; set; }

        public DeviceManagerContext(DbContextOptions<DeviceManagerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConvertDateTimesToUtc();

            builder.ApplyConfiguration(new DeviceConfiguration());
            builder.ApplyConfiguration(new LocationConfiguration());
            builder.ApplyConfiguration(new SensorConfiguration());
            builder.ApplyConfiguration(new SensorTypeConfiguration());
        }
    }
}