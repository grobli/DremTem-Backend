using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Queries
{
    public class GetLocationQuery : IRequest<LocationDtoExtended>
    {
        public GenericGetRequest QueryParameters { get; }

        public GetLocationQuery(GenericGetRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}