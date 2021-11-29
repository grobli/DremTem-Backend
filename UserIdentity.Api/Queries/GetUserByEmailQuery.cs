using MediatR;
using UserIdentity.Core.Proto;

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