namespace ClientApiGateway.Api.Resources.SensorType
{
    public record UpdateSensorTypeResource
    (
        string Name,
        string DataType,
        string Unit,
        string UnitShort,
        string UnitSymbol,
        bool IsDiscrete,
        bool IsSummable
    );
}