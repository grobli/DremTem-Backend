using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class UpdateSensorCommand : IRequest<SensorDto>
    {
        public UpdateSensorRequest Body { get; }

        public UpdateSensorCommand(UpdateSensorRequest body)
        {
            Body = body;
        }
    }
}