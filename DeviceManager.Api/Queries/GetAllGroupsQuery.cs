using MediatR;
using Shared.Proto.Common;
using Shared.Proto.Group;

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