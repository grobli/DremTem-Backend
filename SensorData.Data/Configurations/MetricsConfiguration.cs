using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensorData.Core.Models;

namespace SensorData.Data.Configurations
{
    public class MetricDailyConfiguration : IEntityTypeConfiguration<MetricBaseDaily>
    {
        public void Configure(EntityTypeBuilder<MetricBaseDaily> builder)
        {
            builder
                .HasNoKey()
                .ToView("reading_metrics_daily_summary");
        }
    }

    public class MetricHourlyConfiguration : IEntityTypeConfiguration<MetricBaseHourly>
    {
        public void Configure(EntityTypeBuilder<MetricBaseHourly> builder)
        {
            builder
                .HasNoKey()
                .ToView("reading_metrics_hourly_summary");
        }
    }
}