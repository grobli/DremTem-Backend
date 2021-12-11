using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto;

namespace DeviceManager.Api.Commands
{
    public class CreateLocationCommand : IRequest<LocationDto>
    {
        public CreateLocationRequest Body { get; }

        public CreateLocationCommand(CreateLocationRequest body)
        {
            Body = body;
        }
    }

    public class DeleteLocationCommand : IRequest<Empty>
    {
        public GenericDeleteRequest Body { get; }

        public DeleteLocationCommand(GenericDeleteRequest body)
        {
            Body = body;
        }
    }

    public class UpdateLocationCommand : IRequest<LocationDto>
    {
        public UpdateLocationRequest Body { get; }

        public UpdateLocationCommand(UpdateLocationRequest body)
        {
            Body = body;
        }
    }
}