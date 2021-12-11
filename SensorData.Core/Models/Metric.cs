using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SensorData.Core.Models
{
    [Keyless]
    public abstract record Metric
    {
        [Column("bucket")] public DateTime TimeBucket { get; set; }
        [Column("sensor_id")] public int SensorId { get; set; }
        [Column("max_value")] public double Max { get; set; }
        [Column("min_value")] public double Min { get; set; }
        [Column("avg_value")] public double Average { get; set; }
        [Column("sum_value")] public double Sum { get; set; }
        [Column("stddev")] public double StdDev { get; set; }
        [Column("variance")] public double Variance { get; set; }
        [Column("num_vals")] public int NumberOfValues { get; set; }
    };

    public sealed record MetricDaily : Metric;

    public sealed record MetricHourly : Metric;

    public enum MetricMode
    {
        Daily,
        Hourly
    }
}