using System;
using DeviceGrpcService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            ConvertDateTimesToUtc();
            
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

            void ConvertDateTimesToUtc()
            {
                var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

                var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                    v => v.HasValue ? v.Value.ToUniversalTime() : v,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(DateTime))
                        {
                            property.SetValueConverter(dateTimeConverter);
                        }
                        else if (property.ClrType == typeof(DateTime?))
                        {
                            property.SetValueConverter(nullableDateTimeConverter);
                        }
                    }
                }
            }
        }
    }
}