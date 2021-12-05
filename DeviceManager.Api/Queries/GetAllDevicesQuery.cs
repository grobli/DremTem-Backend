using MediatR;
using Shared.Proto.Common;
using Shared.Proto.Device;

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