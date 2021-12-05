using MediatR;
using Shared.Proto;
using Shared.Proto.Common;

namespace DeviceManager.Api.Queries
{
    public class GetDeviceQuery : IRequest<DeviceExtendedDto>
    {
        public GenericGetRequest QueryParameters { get; }

        public GetDeviceQuery(GenericGetRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}