using MediatR;
using Shared.Proto.Common;
using Shared.Proto.Location;

namespace DeviceManager.Api.Queries
{
    public class GetAllLocationsQuery : IRequest<GetAllLocationsResponse>
    {
        public GenericGetManyRequest QueryParameters { get; }

        public GetAllLocationsQuery(GenericGetManyRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}