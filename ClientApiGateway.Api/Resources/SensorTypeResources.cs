namespace ClientApiGateway.Api.Resources
{
    public record UpdateSensorTypeResource
    (
        string Name,
        string Unit,
        string UnitShort,
        string UnitSymbol,
        bool IsDiscrete,
        bool IsSummable
    );
}