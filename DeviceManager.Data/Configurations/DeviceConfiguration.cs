using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public const int NameMaxLength = 100;
        public const int DisplayNameMaxLenght = 150;
        public const int DateTimePrecision = 0;
        public const int MacAddressMaxLength = 17;
        public const int ModelMaxLength = 100;
        public const int ManufacturerMaxLength = 100;

        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder
                .HasKey(d => d.Id);

            builder
                .HasIndex(d => d.MacAddress)
                .IsUnique();

            builder
                .HasAlternateKey(d => new { d.Name, d.UserId });

            builder
                .Property(d => d.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(d => d.UserId)
                .IsRequired();

            builder
                .Property(d => d.Name)
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            builder
                .Property(d => d.DisplayName)
                .HasMaxLength(DisplayNameMaxLenght);

            builder
                .Property(d => d.Online)
                .IsRequired();

            builder
                .Property(d => d.LastSeen)
                .HasPrecision(DateTimePrecision);

            builder
                .Property(d => d.LastModified)
                .HasPrecision(DateTimePrecision);

            builder
                .Property(d => d.Created)
                .IsRequired()
                .HasPrecision(DateTimePrecision);

            builder
                .HasOne(d => d.Location)
                .WithMany(l => l.Devices)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .Property(d => d.MacAddress)
                .HasMaxLength(MacAddressMaxLength);

            builder
                .Property(d => d.Manufacturer)
                .HasMaxLength(ManufacturerMaxLength);

            builder
                .Property(d => d.Model)
                .HasMaxLength(ModelMaxLength);
        }
    }
}