using MediatR;
using Shared.Proto.User;

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