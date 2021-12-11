using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto;

namespace SensorData.Api.Commands
{
    public class CreateReadingCommand : IRequest<Empty>
    {
        public CreateReadingCommand(CreateReadingRequest body)
        {
            Body = body;
        }

        public CreateReadingRequest Body { get; }
    }
}