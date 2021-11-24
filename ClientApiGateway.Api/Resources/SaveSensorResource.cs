namespace ClientApiGateway.Api.Resources
{
    public record SaveSensorResource
    (
        string Name,
        int DeviceId,
        string DisplayName,
        int TypeId
    );
}