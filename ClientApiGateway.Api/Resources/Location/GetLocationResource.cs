using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Shared.Proto;

namespace ClientApiGateway.Api.Resources.Location
{
    public class GetLocationResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Timestamp Created { get; set; }
        public Timestamp LastModified { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public IEnumerable<DeviceDto> Devices { get; set; }
    };
}