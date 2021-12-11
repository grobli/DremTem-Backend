using Google.Protobuf.WellKnownTypes;
using MediatR;
using Shared.Proto;

namespace SensorData.Api.Commands
{
    public class DeleteManyReadingsCommand : IRequest<Empty>
    {
        public DeleteManyReadingsCommand(DeleteManyReadingsRequest body)
        {
            Body = body;
        }

        public DeleteManyReadingsRequest Body { get; }
    }
}