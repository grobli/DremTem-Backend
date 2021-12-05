using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto.UserIdentity;

namespace UserIdentity.Api.Commands
{
    public class CreateRoleCommand : IRequest<Empty>
    {
        public CreateRoleCommand(CreateRoleRequest body)
        {
            Body = body;
        }

        public CreateRoleRequest Body { get; }
    }
}