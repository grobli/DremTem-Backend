using DeviceManager.Core.Proto;
using MediatR;

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