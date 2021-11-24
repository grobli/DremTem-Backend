using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public const int NameMaxLength = 40;
        public const int DisplayNameMaxLength = 150;
        public const int DateTimePrecision = 0;

        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder
                .HasKey(l => l.Id);

            builder
                .HasAlternateKey(l => new { l.Name, l.UserId });

            builder
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder
                .Property(l => l.DisplayName)
                .HasMaxLength(DisplayNameMaxLength);

            builder
                .Property(l => l.LastModified)
                .HasPrecision(DateTimePrecision);

            builder
                .Property(l => l.Created)
                .IsRequired()
                .HasPrecision(DateTimePrecision);

            builder
                .HasCheckConstraint(
                    "ch_latitude_longitude_both_defined_or_undefined",
                    "(latitude IS NULL AND longitude IS NULL) OR " +
                    "(latitude IS NOT NULL AND longitude IS NOT NULL)");
        }
    }
}