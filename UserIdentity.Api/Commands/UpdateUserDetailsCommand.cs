using MediatR;
using UserIdentity.Core.Proto;

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