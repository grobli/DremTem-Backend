using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensorData.Core.Models;

namespace SensorData.Data.Configurations
{
    public class ReadingConfiguration : IEntityTypeConfiguration<Reading>
    {
        public void Configure(EntityTypeBuilder<Reading> builder)
        {
            builder
                .HasKey(r => new { r.Time, r.SensorId });

            builder
                .Property(r => r.Value)
                .IsRequired();
        }
    }
}