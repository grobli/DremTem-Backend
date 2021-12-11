using System.Collections.Generic;

namespace ClientApiGateway.Api.Resources.Reading.Metric
{
    public class GetMetricsResource
    {
        public int SensorId { get; set; }
        public int SensorTypeId { get; set; }
        public IEnumerable<MetricNoSensorIdResource> Metrics { get; set; }
    }
}