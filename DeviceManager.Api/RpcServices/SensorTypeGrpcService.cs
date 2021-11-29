using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Messages;
using DeviceManager.Core.Proto;
using EasyNetQ;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Api.RpcServices
{
    public class SensorTypeGrpcService : Core.Proto.SensorTypeGrpcService.SensorTypeGrpcServiceBase
    {
        private readonly ILogger<SensorTypeGrpcService> _logger;
        private readonly IMediator _mediator;

        public SensorTypeGrpcService(ILogger<SensorTypeGrpcService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<GetAllSensorTypesResponse> GetAllSensorTypes(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var query = new GetAllSensorTypesQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<SensorTypeDto> GetSensorType(GenericGetRequest request,
            ServerCallContext context)
        {
            var query = new GetSensorTypeQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<SensorTypeDto> CreateSensorType(CreateSensorTypeRequest request,
            ServerCallContext context)
        {
            var command = new CreateSensorTypeCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<SensorTypeDto> UpdateSensorType(UpdateSensorTypeRequest request,
            ServerCallContext context)
        {
            var command = new UpdateSensorTypeCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<DeleteSensorTypeResponse> DeleteSensorType(GenericDeleteRequest request,
            ServerCallContext context)
        {
            var command = new DeleteSensorTypeCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }
    }
}