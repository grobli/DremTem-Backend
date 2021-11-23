namespace ClientApiGateway.Api.Resources
{
    public record SaveSensorResource
    (
        string Name,
        long DeviceId,
        string DisplayName,
        string TypeName
    );
}