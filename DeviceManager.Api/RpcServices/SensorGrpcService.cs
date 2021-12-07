using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Api.Queries;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Sensor;

namespace DeviceManager.Api.RpcServices
{
    public class SensorGrpcService : SensorGrpc.SensorGrpcBase
    {
        private readonly ILogger<SensorGrpcService> _logger;
        private readonly IMediator _mediator;

        public SensorGrpcService(ILogger<SensorGrpcService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<GetAllSensorsResponse> GetAllSensors(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var query = new GetAllSensorsQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<SensorDto> GetSensor(GenericGetRequest request, ServerCallContext context)
        {
            var query = new GetSensorQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<SensorDto> GetSensorByName(GetSensorByNameRequest request, ServerCallContext context)
        {
            var query = new GetSensorByNameQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<SensorDto> AddSensor(CreateSensorRequest request, ServerCallContext context)
        {
            var command = new CreateSensorCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<SensorDto> UpdateSensor(UpdateSensorRequest request, ServerCallContext context)
        {
            var command = new UpdateSensorCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<DeleteSensorResponse> DeleteSensor(GenericDeleteRequest request,
            ServerCallContext context)
        {
            var command = new DeleteSensorCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }
    }
}