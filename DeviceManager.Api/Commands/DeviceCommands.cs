using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Device;

namespace DeviceManager.Api.Commands
{
    public class CreateDeviceCommand : IRequest<DeviceExtendedDto>
    {
        public CreateDeviceRequest Body { get; }

        public CreateDeviceCommand(CreateDeviceRequest body)
        {
            Body = body;
        }
    }

    public class DeleteDeviceCommand : IRequest<DeleteDeviceResponse>
    {
        public GenericDeleteRequest Body { get; }

        public DeleteDeviceCommand(GenericDeleteRequest body)
        {
            Body = body;
        }
    }

    public class UpdateDeviceCommand : IRequest<DeviceDto>
    {
        public UpdateDeviceRequest Body { get; }

        public UpdateDeviceCommand(UpdateDeviceRequest body)
        {
            Body = body;
        }
    }

    public class PingCommand : IRequest<Empty>
    {
        public PingRequest Body { get; }

        public PingCommand(PingRequest body)
        {
            Body = body;
        }
    }

    public class GenerateTokenCommand : IRequest<GenerateTokenResponse>
    {
        public GenerateTokenRequest Body { get; }

        public GenerateTokenCommand(GenerateTokenRequest body)
        {
            Body = body;
        }
    }
}