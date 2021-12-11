using System;
using Shared;

namespace SensorData.Core.Models
{
    public record Reading
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public int SensorId { get; set; }
    }

    public class ReadingParameters : QueryStringParameters
    {
    }

    public class ReadingPagedParameters : ReadingParameters
    {
        public PaginationParameters Page { get; } = new();
    }
}