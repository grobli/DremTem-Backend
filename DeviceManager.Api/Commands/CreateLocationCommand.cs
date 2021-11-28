using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class CreateLocationCommand : IRequest<LocationDto>
    {
        public CreateLocationRequest Body { get; }

        public CreateLocationCommand(CreateLocationRequest body)
        {
            Body = body;
        }
    }
}