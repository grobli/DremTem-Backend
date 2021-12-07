using System;

namespace ClientApiGateway.Api.Resources.Reading
{
    public class ReadingNoSensorResource
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
    }
}