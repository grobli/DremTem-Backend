namespace ClientApiGateway.Api.Resources
{
    public record UpdateLocationResource
    (
        string DisplayName,
        float? Latitude,
        float? Longitude
    );
}