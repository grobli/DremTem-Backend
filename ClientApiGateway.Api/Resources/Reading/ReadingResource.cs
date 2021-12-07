using System;

namespace ClientApiGateway.Api.Resources.Reading
{
    public class ReadingResource
    {
        public DateTime Time { get; set; }
        public int SensorId { get; set; }
        public double Value { get; set; }
    }
}