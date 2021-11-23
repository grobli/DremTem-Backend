namespace ClientApiGateway.Api.Resources
{
    public record UpdateDeviceResource
    (
        string DisplayName,
        bool Online,
        string LocationName
    );
}