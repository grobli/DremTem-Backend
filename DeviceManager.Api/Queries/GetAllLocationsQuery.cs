using DeviceManager.Core.Proto;
using MediatR;

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