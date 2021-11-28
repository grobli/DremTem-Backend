using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class UpdateLocationCommand : IRequest<LocationDto>
    {
        public UpdateLocationRequest Body { get; }

        public UpdateLocationCommand(UpdateLocationRequest body)
        {
            Body = body;
        }
    }
}