using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto;

namespace SensorData.Api.Commands
{
    public class DeleteReadingCommand : IRequest<Empty>
    {
        public DeleteReadingCommand(DeleteReadingRequest body)
        {
            Body = body;
        }

        public DeleteReadingRequest Body { get; }
    }
}