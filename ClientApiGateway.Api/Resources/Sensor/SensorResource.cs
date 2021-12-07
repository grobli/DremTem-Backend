using System;
using Shared.Proto;

namespace ClientApiGateway.Api.Resources.Sensor
{
    public class SensorResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int DeviceId { get; set; }
        public int TypeId { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public SensorTypeDto Type { get; set; }
    }
}