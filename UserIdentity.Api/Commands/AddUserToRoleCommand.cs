using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto.UserIdentity;

namespace UserIdentity.Api.Commands
{
    public class AddUserToRoleCommand : IRequest<Empty>
    {
        public AddUserToRoleCommand(AddUserToRoleRequest body)
        {
            Body = body;
        }

        public AddUserToRoleRequest Body { get; }
    }
}