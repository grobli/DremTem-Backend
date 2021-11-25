namespace ClientApiGateway.Api.Resources.Location
{
    public record CreateLocationResource
    (
        string Name,
        string DisplayName,
        float? Latitude,
        float? Longitude
    );
}