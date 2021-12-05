using MediatR;
using Shared.Proto;
using Shared.Proto.Common;

namespace DeviceManager.Api.Queries
{
    public class GetSensorQuery : IRequest<SensorDto>
    {
        public GenericGetRequest QueryParameters { get; }

        public GetSensorQuery(GenericGetRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}