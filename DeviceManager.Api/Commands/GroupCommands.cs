using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Group;

namespace DeviceManager.Api.Commands
{
    public class CreateGroupCommand : IRequest<GroupDto>
    {
        public CreateGroupRequest Body { get; }

        public CreateGroupCommand(CreateGroupRequest body)
        {
            Body = body;
        }
    }

    public class DeleteGroupCommand : IRequest<Empty>
    {
        public GenericDeleteRequest Body { get; }

        public DeleteGroupCommand(GenericDeleteRequest body)
        {
            Body = body;
        }
    }

    public class UpdateGroupCommand : IRequest<GroupDto>
    {
        public UpdateGroupRequest Body { get; }

        public UpdateGroupCommand(UpdateGroupRequest body)
        {
            Body = body;
        }
    }

    public class AddDeviceToGroupCommand : IRequest<Empty>
    {
        public AddDeviceToGroupRequest Body { get; }

        public AddDeviceToGroupCommand(AddDeviceToGroupRequest body)
        {
            Body = body;
        }
    }

    public class RemoveDeviceFromGroupCommand : IRequest<Empty>
    {
        public RemoveDeviceFromGroupRequest Body { get; }

        public RemoveDeviceFromGroupCommand(RemoveDeviceFromGroupRequest body)
        {
            Body = body;
        }
    }
}