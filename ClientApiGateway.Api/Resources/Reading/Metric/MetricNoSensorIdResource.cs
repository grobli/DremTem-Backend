using System;

namespace ClientApiGateway.Api.Resources.Reading.Metric
{
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