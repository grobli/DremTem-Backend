namespace ClientApiGateway.Api.Resources.SensorType
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