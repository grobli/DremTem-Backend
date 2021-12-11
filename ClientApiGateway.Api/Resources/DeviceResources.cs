using System;
using System.Collections.Generic;
using Shared.Proto;

namespace ClientApiGateway.Api.Resources
{
    public record CreateDeviceResource
    (
        string Name,
        string DisplayName,
        bool Online,
        string MacAddress,
        string Model,
        string Manufacturer,
        int? LocationId,
        IEnumerable<CreateDeviceSensorResource> Sensors
    );

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

    public record UpdateDeviceResource
    (
        string DisplayName,
        bool Online,
        string Model,
        string Manufacturer,
        string MacAddress,
        int? LocationId
    );
}