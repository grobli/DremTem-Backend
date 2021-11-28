using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class UpdateDeviceCommand : IRequest<DeviceDto>
    {
        public UpdateDeviceRequest Body { get; }

        public UpdateDeviceCommand(UpdateDeviceRequest body)
        {
            Body = body;
        }
    }
}