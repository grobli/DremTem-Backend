using System.Collections.Generic;

namespace ClientApiGateway.Api.Resources.Reading
{
    public class GetReadingsFromSensorResource
    {
        public int SensorId { get; set; }
        public int SensorTypeId { get; set; }
        public IEnumerable<ReadingNoSensorResource> Readings { get; set; }
    }
}