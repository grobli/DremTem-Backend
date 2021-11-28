using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class CreateDeviceCommand : IRequest<DeviceDto>
    {
        public CreateDeviceRequest Body { get; }

        public CreateDeviceCommand(CreateDeviceRequest body)
        {
            Body = body;
        }
    }
}