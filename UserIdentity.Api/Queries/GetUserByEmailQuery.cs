using MediatR;
using Shared.Proto;

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