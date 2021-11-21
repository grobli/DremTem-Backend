using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class SensorTypeConfiguration : IEntityTypeConfiguration<SensorType>
    {
        public void Configure(EntityTypeBuilder<SensorType> builder)
        {
            builder
                .HasKey(st => st.Name);

            builder
                .Property(st => st.DataType)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(st => st.Unit)
                .IsRequired()
                .HasMaxLength(64);

            builder
                .Property(st => st.UnitShort)
                .IsRequired()
                .HasMaxLength(32);

            builder
                .Property(st => st.UnitSymbol)
                .HasMaxLength(16);

            builder
                .Property(st => st.IsDiscrete)
                .IsRequired();

            builder
                .Property(st => st.IsSummable)
                .IsRequired();
        }
    }
}