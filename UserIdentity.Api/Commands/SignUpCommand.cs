using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto.UserIdentity;

namespace UserIdentity.Api.Commands
{
    public class SignUpCommand : IRequest<Empty>
    {
        public SignUpCommand(UserSignUpRequest body)
        {
            Body = body;
        }

        public UserSignUpRequest Body { get; }
    }
}