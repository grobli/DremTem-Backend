﻿using DeviceManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeviceManager.Data.Configurations
{
    public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
    {
        public const int NameMaxLength = 100;
        public const int DisplayNameMaxLength = 150;
        public const int DateTimePrecision = 0;

        public void Configure(EntityTypeBuilder<Sensor> builder)
        {
            builder
                .HasKey(s => s.Id);

            builder
                .HasAlternateKey(s => new { s.Name, s.DeviceId });

            builder
                .HasIndex(s => new { s.Name, s.DeviceId })
                .IsUnique();

            builder
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(s => s.Name)
                .HasMaxLength(NameMaxLength)
                .IsRequired();

            builder
                .Property(s => s.DisplayName)
                .HasMaxLength(DisplayNameMaxLength);

            builder
                .HasOne(s => s.Device)
                .WithMany(d => d.Sensors)
                .HasForeignKey(s => s.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(s => s.Type)
                .WithMany(st => st.Sensors)
                .HasForeignKey(s => s.TypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

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