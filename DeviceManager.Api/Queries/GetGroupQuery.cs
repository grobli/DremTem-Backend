using MediatR;
using Shared.Proto;
using Shared.Proto.Common;

namespace DeviceManager.Api.Queries
{
    public class GetGroupQuery : IRequest<GroupDto>
    {
        public GenericGetRequest QueryParameters { get; }

        public GetGroupQuery(GenericGetRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}