using MediatR;
using Shared.Proto;

namespace UserIdentity.Api.Queries
{
    public class GetAllUsersQuery : IRequest<GetAllUsersResponse>
    {
        public GetAllUsersQuery(GetAllUsersRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }

        public GetAllUsersRequest QueryParameters { get; }
    }
}