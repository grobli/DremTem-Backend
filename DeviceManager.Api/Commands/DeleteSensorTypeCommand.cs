using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class DeleteSensorTypeCommand : IRequest<Empty>
    {
        public GenericDeleteRequest Body { get; }

        public DeleteSensorTypeCommand(GenericDeleteRequest body)
        {
            Body = body;
        }
    }
}