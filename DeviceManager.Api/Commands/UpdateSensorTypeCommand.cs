using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class UpdateSensorTypeCommand : IRequest<SensorTypeDto>
    {
        public UpdateSensorTypeRequest Body { get; }

        public UpdateSensorTypeCommand(UpdateSensorTypeRequest body)
        {
            Body = body;
        }
    }
}