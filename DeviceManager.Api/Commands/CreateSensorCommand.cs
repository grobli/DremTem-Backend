using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class CreateSensorCommand : IRequest<SensorDto>
    {
        public CreateSensorRequest Body { get; }

        public CreateSensorCommand(CreateSensorRequest body)
        {
            Body = body;
        }
    }
}