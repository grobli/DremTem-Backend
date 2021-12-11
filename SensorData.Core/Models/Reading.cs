using System;

namespace SensorData.Core.Models
{
    public sealed class Reading
    {
        public DateTime Time { get; init; }
        public double Value { get; set; }
        public int SensorId { get; init; }
    }
}