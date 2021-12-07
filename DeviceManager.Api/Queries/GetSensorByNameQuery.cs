using MediatR;
using Shared.Proto;
using Shared.Proto.Sensor;

namespace DeviceManager.Api.Queries
{
    public class GetSensorByNameQuery : IRequest<SensorDto>
    {
        public GetSensorByNameRequest QueryParameters { get; }

        public GetSensorByNameQuery(GetSensorByNameRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}