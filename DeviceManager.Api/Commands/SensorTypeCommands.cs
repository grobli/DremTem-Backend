using MediatR;
using Shared.Proto;

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

    public class DeleteSensorTypeCommand : IRequest<DeleteSensorTypeResponse>
    {
        public GenericDeleteRequest Body { get; }

        public DeleteSensorTypeCommand(GenericDeleteRequest body)
        {
            Body = body;
        }
    }

    public class UpdateSensorTypeCommand : IRequest<SensorTypeDto>
    {
        public UpdateSensorTypeRequest Body { get; }

        public UpdateSensorTypeCommand(UpdateSensorTypeRequest body)
        {
            Body = body;
        }
    }
}