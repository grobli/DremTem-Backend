using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class SensorTypeConfiguration : IEntityTypeConfiguration<SensorType>
    {
        public const int DateTimePrecision = 0;
        public const int NameMaxLength = 40;
        public const int UnitMaxLength = 64;
        public const int UnitShortMaxLength = 32;
        public const int UnitSymbolMaxLength = 16;
        public const int DisplayNameMaxLength = 150;

        public void Configure(EntityTypeBuilder<SensorType> builder)
        {
            builder
                .HasKey(st => st.Id);

            builder
                .HasAlternateKey(st => st.Name);

            builder
                .Property(st => st.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(st => st.Name)
                .IsRequired()
                .HasMaxLength(NameMaxLength);

            builder
                .Property(s => s.DisplayName)
                .HasMaxLength(DisplayNameMaxLength);

            builder
                .Property(st => st.Unit)
                .IsRequired()
                .HasMaxLength(UnitMaxLength);

            builder
                .Property(st => st.UnitShort)
                .HasMaxLength(UnitShortMaxLength);

            builder
                .Property(st => st.UnitSymbol)
                .HasMaxLength(UnitSymbolMaxLength);

            builder
                .Property(st => st.IsDiscrete)
                .IsRequired();

            builder
                .Property(st => st.IsSummable)
                .IsRequired();

            builder
                .Property(d => d.LastModified)
                .HasPrecision(DateTimePrecision);

            builder
                .Property(d => d.Created)
                .IsRequired()
                .HasPrecision(DateTimePrecision);
        }
    }
}