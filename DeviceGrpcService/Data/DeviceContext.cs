using DeviceGrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceGrpcService.Data
{
    public class DeviceContext : DbContext
    {
        public DeviceContext(DbContextOptions<DeviceContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorType> SensorTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().ToTable(nameof(Device));
            modelBuilder.Entity<Location>().ToTable(nameof(Location));
            modelBuilder.Entity<Sensor>().ToTable(nameof(Sensor));
            modelBuilder.Entity<SensorType>().ToTable(nameof(SensorType));

            modelBuilder.Entity<Device>()
                .Navigation(d => d.Location)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Device>()
                .Navigation(d => d.Sensors)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}