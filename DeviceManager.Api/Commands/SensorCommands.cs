using MediatR;
using Shared.Proto;

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

    public class UpdateSensorCommand : IRequest<SensorDto>
    {
        public UpdateSensorRequest Body { get; }

        public UpdateSensorCommand(UpdateSensorRequest body)
        {
            Body = body;
        }
    }

    public class DeleteSensorCommand : IRequest<DeleteSensorResponse>
    {
        public DeleteSensorCommand(GenericDeleteRequest body)
        {
            Body = body;
        }

        public GenericDeleteRequest Body { get; }
    }
}