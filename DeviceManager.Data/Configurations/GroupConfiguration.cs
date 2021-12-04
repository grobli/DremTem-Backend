using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public const int NameMaxLength = 100;
        public const int DisplayNameMaxLenght = 150;
        public const int DateTimePrecision = 0;

        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder
                .HasKey(g => g.Id);

            builder
                .HasAlternateKey(g => new { g.Name, g.UserId });

            builder
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(g => g.UserId)
                .IsRequired();

            builder
                .Property(d => d.Name)
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            builder
                .Property(d => d.DisplayName)
                .HasMaxLength(DisplayNameMaxLenght);

            builder
                .Property(d => d.LastModified)
                .HasPrecision(DateTimePrecision);

            builder
                .Property(d => d.Created)
                .IsRequired()
                .HasPrecision(DateTimePrecision);

            builder
                .HasMany(g => g.Devices)
                .WithMany(d => d.Groups);
        }
    }
}