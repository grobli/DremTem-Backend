using MediatR;
using Shared.Proto;

namespace DeviceManager.Api.Queries
{
    public class GetSensorTypeQuery : IRequest<SensorTypeDto>
    {
        public GenericGetRequest QueryParameters { get; }

        public GetSensorTypeQuery(GenericGetRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}