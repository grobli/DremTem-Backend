namespace ClientApiGateway.Api.Resources.Device
{
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