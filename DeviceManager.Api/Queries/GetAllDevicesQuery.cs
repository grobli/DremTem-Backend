using MediatR;
using Shared.Proto;

namespace DeviceManager.Api.Queries
{
    public class GetAllDevicesQuery : IRequest<GetAllDevicesResponse>
    {
        public GenericGetManyRequest QueryParameters { get; }

        public GetAllDevicesQuery(GenericGetManyRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}