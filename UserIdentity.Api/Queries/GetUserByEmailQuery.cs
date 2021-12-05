using MediatR;
using Shared.Proto.User;

namespace UserIdentity.Api.Queries
{
    public class GetUserByEmailQuery : IRequest<UserDto>
    {
        public GetUserByEmailQuery(GetUserByEmailRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }

        public GetUserByEmailRequest QueryParameters { get; }
    }
}