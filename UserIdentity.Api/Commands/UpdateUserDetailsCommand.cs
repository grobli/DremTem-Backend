using MediatR;
using Shared.Proto.User;

namespace UserIdentity.Api.Commands
{
    public class UpdateUserDetailsCommand : IRequest<UserDto>
    {
        public UpdateUserDetailsCommand(UpdateUserDetailsRequest body)
        {
            Body = body;
        }

        public UpdateUserDetailsRequest Body { get; }
    }
}