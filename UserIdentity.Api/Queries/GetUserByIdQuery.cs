using MediatR;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public GetUserByIdQuery(GetUserByIdRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }

        public GetUserByIdRequest QueryParameters { get; }
    }
}