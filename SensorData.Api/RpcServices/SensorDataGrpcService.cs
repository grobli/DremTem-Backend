using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using SensorData.Api.Queries;
using Shared.Proto.SensorData;

namespace SensorData.Api.RpcServices
{
    public class SensorDataGrpcService : SensorDataGrpc.SensorDataGrpcBase
    {
        private readonly ILogger<SensorDataGrpcService> _logger;
        private readonly IMediator _mediator;

        public SensorDataGrpcService(ILogger<SensorDataGrpcService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<GetManyFromSensorResponse> GetAllFromSensor(GetAllFromSensorRequest request,
            ServerCallContext context)
        {
            var query = new GetAllFromSensorQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }
    }
}