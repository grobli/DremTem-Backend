using DeviceManager.Core.Proto;
using MediatR;

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