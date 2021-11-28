using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class PingCommand : IRequest<Empty>
    {
        public PingRequest Body { get; }

        public PingCommand(PingRequest body)
        {
            Body = body;
        }
    }
}