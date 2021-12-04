using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Queries
{
    public class GetAllGroupsQuery : IRequest<GetAllGroupsResponse>
    {
        public GenericGetManyRequest QueryParameters { get; }

        public GetAllGroupsQuery(GenericGetManyRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}