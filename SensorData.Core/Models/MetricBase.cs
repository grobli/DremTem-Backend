using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SensorData.Core.Models
{
    [Keyless]
    public abstract record MetricBase
    {
        [Column("bucket")] public DateTime TimeBucket { get; init; }
        [Column("sensor_id")] public int SensorId { get; init; }
        [Column("max_value")] public double Max { get; init; }
        [Column("min_value")] public double Min { get; init; }
        [Column("avg_value")] public double Average { get; init; }
        [Column("sum_value")] public double Sum { get; init; }
        [Column("stddev")] public double StdDev { get; init; }
        [Column("variance")] public double Variance { get; init; }
        [Column("num_vals")] public int NumberOfValues { get; init; }
    };

    public sealed record MetricBaseDaily : MetricBase;

    public sealed record MetricBaseHourly : MetricBase;


    public enum MetricMode
    {
        Daily,
        Hourly
    }
}