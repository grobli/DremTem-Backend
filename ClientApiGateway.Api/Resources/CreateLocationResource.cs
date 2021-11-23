namespace ClientApiGateway.Api.Resources
{
    public record CreateLocationResource
    (
        string Name,
        string DisplayName,
        float? Latitude,
        float? Longitude
    );
}