using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Api.RpcServices
{
    public class DeviceGrpcService : Core.Proto.DeviceGrpcService.DeviceGrpcServiceBase
    {
        private readonly ILogger<DeviceGrpcService> _logger;
        private readonly IMediator _mediator;

        public DeviceGrpcService(ILogger<DeviceGrpcService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<GetAllDevicesResponse> GetAllDevices(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var query = new GetAllDevicesQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<DeviceDtoExtended> GetDevice(GenericGetRequest request,
            ServerCallContext context)
        {
            var query = new GetDeviceQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<DeviceDto> CreateDevice(CreateDeviceRequest request, ServerCallContext context)
        {
            var command = new CreateDeviceCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<DeviceDto> UpdateDevice(UpdateDeviceRequest request, ServerCallContext context)
        {
            var command = new UpdateDeviceCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override Task<Empty> DeleteDevice(GenericDeleteRequest request, ServerCallContext context)
        {
            /* TODO: Tutaj trochę skomplikowane bo trzeba nie tylko usunąć samo urządzenie ale również sensory
             TODO: i dane zebrane z sensorów w innym serwisie (bo nastąpi desynchronizacja danych AAAAAAAA) */
            return base.DeleteDevice(request, context);
        }

        public override async Task<Empty> Ping(PingRequest request, ServerCallContext context)
        {
            var command = new PingCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request,
            ServerCallContext context)
        {
            var command = new GenerateTokenCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }
    }
}