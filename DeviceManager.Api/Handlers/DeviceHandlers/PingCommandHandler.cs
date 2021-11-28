using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class PingCommandHandler : IRequestHandler<PingCommand, Empty>
    {
        private readonly IDeviceService _deviceService;

        public PingCommandHandler(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public async Task<Empty> Handle(PingCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceService.GetDeviceQuery(request.Body.Id)
                .SingleOrDefaultAsync(cancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _deviceService.UpdateDeviceLastSeenAsync(device, cancellationToken);
            return new Empty();
        }
    }
}