using MediatR;
using Shared.Proto;

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