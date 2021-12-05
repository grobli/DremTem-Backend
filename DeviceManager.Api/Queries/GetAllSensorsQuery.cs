using MediatR;
using Shared.Proto.Common;
using Shared.Proto.Sensor;

namespace DeviceManager.Api.Queries
{
    public class GetAllSensorsQuery : IRequest<GetAllSensorsResponse>
    {
        public GenericGetManyRequest QueryParameters { get; }

        public GetAllSensorsQuery(GenericGetManyRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}