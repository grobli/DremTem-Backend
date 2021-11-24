namespace ClientApiGateway.Api.Resources
{
    public record UpdateDeviceResource
    (
        string DisplayName,
        bool Online,
        string Model,
        string Manufacturer,
        int? LocationId
    );
}