using MediatR;
using Shared.Proto.User;

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