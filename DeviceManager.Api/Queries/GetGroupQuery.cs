using DeviceManager.Core.Proto;
using MediatR;

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