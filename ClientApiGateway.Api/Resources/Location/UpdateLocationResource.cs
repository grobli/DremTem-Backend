namespace ClientApiGateway.Api.Resources.Location
{
    public record UpdateLocationResource
    (
        string DisplayName,
        float? Latitude,
        float? Longitude
    );
}