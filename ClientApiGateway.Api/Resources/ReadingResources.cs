using System;
using System.Collections.Generic;

namespace ClientApiGateway.Api.Resources
{
    public class GetReadingsFromSensorResource
    {
        public int SensorId { get; set; }
        public int SensorTypeId { get; set; }
        public IEnumerable<ReadingNoSensorResource> Readings { get; set; }
    }

    public class ReadingNoSensorResource
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
    }

    public class ReadingResource
    {
        public DateTime Time { get; set; }
        public int SensorId { get; set; }
        public double Value { get; set; }
    }

    public class GetMetricsResource
    {
        public int SensorId { get; set; }
        public int SensorTypeId { get; set; }
        public IEnumerable<MetricNoSensorIdResource> Metrics { get; set; }
    }

    public class MetricNoSensorIdResource
    {
        public DateTime TimeBucket { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Average { get; set; }
        public double Sum { get; set; }
        public double StdDev { get; set; }
        public double Variance { get; set; }
        public int NumberOfValues { get; set; }
    }
}