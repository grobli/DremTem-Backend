using MediatR;
using Shared.Proto;
using Shared.Proto.Common;

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