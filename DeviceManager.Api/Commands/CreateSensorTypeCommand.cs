using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class CreateSensorTypeCommand : IRequest<SensorTypeDto>
    {
        public CreateSensorTypeRequest Body { get; }

        public CreateSensorTypeCommand(CreateSensorTypeRequest body)
        {
            Body = body;
        }
    }
}