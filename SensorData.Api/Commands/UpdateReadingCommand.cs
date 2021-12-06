using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto.SensorData;

namespace SensorData.Api.Commands
{
    public class UpdateReadingCommand : IRequest<Empty>
    {
        public UpdateReadingCommand(UpdateReadingRequest body)
        {
            Body = body;
        }

        public UpdateReadingRequest Body { get; }
    }
}