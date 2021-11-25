namespace ClientApiGateway.Api.Resources.Sensor
{
    public record SaveSensorResource
    (
        string Name,
        int DeviceId,
        string DisplayName,
        int TypeId
    );
}