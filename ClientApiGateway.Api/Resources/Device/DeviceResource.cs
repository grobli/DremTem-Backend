using System;
using System.Collections.Generic;
using ClientApiGateway.Api.Resources.Location;
using ClientApiGateway.Api.Resources.Sensor;
using Shared.Proto;

namespace ClientApiGateway.Api.Resources.Device
{
    public class DeviceResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Online { get; set; }
        public string MacAddress { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public int? LocationId { get; set; }
        public string UserId { get; set; }
        public IEnumerable<int> SensorIds { get; set; }
        public IEnumerable<GroupTinyDto> Groups { get; set; }
        public IEnumerable<SensorResource> Sensors { get; set; }
        public LocationResource Location { get; set; }
    }
}