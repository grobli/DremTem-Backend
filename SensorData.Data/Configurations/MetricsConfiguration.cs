using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensorData.Core.Models;

namespace SensorData.Data.Configurations
{
    public class MetricDailyConfiguration : IEntityTypeConfiguration<MetricDaily>
    {
        public void Configure(EntityTypeBuilder<MetricDaily> builder)
        {
            builder
                .HasNoKey()
                .ToView("reading_metrics_daily_summary");
        }
    }

    public class MetricHourlyConfiguration : IEntityTypeConfiguration<MetricHourly>
    {
        public void Configure(EntityTypeBuilder<MetricHourly> builder)
        {
            builder
                .HasNoKey()
                .ToView("reading_metrics_hourly_summary");
        }
    }
}