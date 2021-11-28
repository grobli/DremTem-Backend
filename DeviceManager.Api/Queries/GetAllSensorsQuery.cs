using DeviceManager.Core.Proto;
using MediatR;

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