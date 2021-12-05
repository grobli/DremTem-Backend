using MediatR;
using Shared.Proto;
using Shared.Proto.Common;

namespace DeviceManager.Api.Queries
{
    public class GetLocationQuery : IRequest<LocationExtendedDto>
    {
        public GenericGetRequest QueryParameters { get; }

        public GetLocationQuery(GenericGetRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}