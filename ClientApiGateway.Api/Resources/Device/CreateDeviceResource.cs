using System.Collections.Generic;
using DeviceManager.Core.Proto;

namespace ClientApiGateway.Api.Resources.Device
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
}