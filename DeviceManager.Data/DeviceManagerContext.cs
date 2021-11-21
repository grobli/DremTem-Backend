using System;
using DeviceManager.Core.Models;
using DeviceManager.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

            ConvertDateTimesToUtc();

            builder.ApplyConfiguration(new DeviceConfiguration());
            builder.ApplyConfiguration(new LocationConfiguration());
            builder.ApplyConfiguration(new SensorConfiguration());
            builder.ApplyConfiguration(new SensorTypeConfiguration());

            void ConvertDateTimesToUtc()
            {
                var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

                var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                    v => v.HasValue ? v.Value.ToUniversalTime() : v,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

                foreach (var entityType in builder.Model.GetEntityTypes())
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