using DeviceManager.Core.Proto;
using MediatR;

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