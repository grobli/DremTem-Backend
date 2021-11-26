using System;

namespace SensorData.Core.Models
{
    public record ReadingBucket
    {
        public DateTime TimeBucket { get; set; }
        public double AvgValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public int SensorId { get; set; }
    }
}