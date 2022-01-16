using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto;

namespace UserIdentity.Api.Commands
{
    public class ChangePasswordCommand : IRequest<Empty>
    {
        public ChangePasswordCommand(ChangePasswordRequest body)
        {
            Body = body;
        }

        public ChangePasswordRequest Body { get; }
    }
}