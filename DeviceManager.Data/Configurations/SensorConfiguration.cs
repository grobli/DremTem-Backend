using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
    {
        public const int NameMaxLength = 100;
        public const int DisplayNameMaxLength = 150;

        public void Configure(EntityTypeBuilder<Sensor> builder)
        {
            builder
                .HasKey(s => new { s.UserId, s.DeviceName, s.Name });

            builder
                .Property(s => s.Name)
                .HasMaxLength(NameMaxLength);

            builder
                .Property(s => s.DisplayName)
                .HasMaxLength(DisplayNameMaxLength);

            builder
                .Property(s => s.DeviceName)
                .HasMaxLength(DeviceConfiguration.NameMaxLength);

            builder
                .Property(s => s.TypeName)
                .HasMaxLength(100);

            builder
                .HasOne(s => s.Device)
                .WithMany(d => d.Sensors)
                .HasForeignKey(s => new { s.UserId, s.DeviceName })
                .IsRequired();

            builder
                .HasOne(s => s.Type)
                .WithMany(st => st.Sensors)
                .HasForeignKey(s => s.TypeName)
                .IsRequired();
        }
    }
}