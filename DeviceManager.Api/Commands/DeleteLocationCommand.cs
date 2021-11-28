using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class DeleteLocationCommand : IRequest<Empty>
    {
        public GenericDeleteRequest Body { get; }

        public DeleteLocationCommand(GenericDeleteRequest body)
        {
            Body = body;
        }
    }
}