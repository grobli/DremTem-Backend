using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public const int NameMaxLength = 100;
        public const int DisplayNameMaxLength = 150;
        public const int DateTimePrecision = 0;

        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder
                .HasKey(l => new { l.UserId, l.Name });

            builder
                .Property(l => l.Name)
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
        }
    }
}