using System;
using System.Collections.Generic;
using ClientApiGateway.Api.Resources.Device;
using Shared.Proto;

namespace ClientApiGateway.Api.Resources.Location
{
    public class LocationResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public IEnumerable<int> DeviceIds { get; set; }
        public IEnumerable<DeviceResource> Devices { get; set; }
    };
}