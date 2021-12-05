using MediatR;
using Shared.Proto.Common;
using Shared.Proto.SensorType;

namespace DeviceManager.Api.Queries
{
    public class GetAllSensorTypesQuery : IRequest<GetAllSensorTypesResponse>
    {
        public GenericGetManyRequest QueryParameter { get; }

        public GetAllSensorTypesQuery(GenericGetManyRequest queryParameter)
        {
            QueryParameter = queryParameter;
        }
    }
}