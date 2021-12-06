using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using SensorData.Api.Commands;
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

        public override async Task<ReadingDto> GetFirstRecentFromSensor(GetFirstRecentFromSensorRequest request,
            ServerCallContext context)
        {
            var query = new GetFirstReadingFromSensorQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<GetManyFromSensorResponse> GetAllFromSensor(GetAllFromSensorRequest request,
            ServerCallContext context)
        {
            var query = new GetAllFromSensorQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<GetManyFromSensorResponse> GetLastFromSensor(GetLastFromSensorRequest request,
            ServerCallContext context)
        {
            var query = new GetLastFromSensorQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<GetManyFromSensorResponse> GetRangeFromSensor(GetRangeFromSensorRequest request,
            ServerCallContext context)
        {
            var query = new GetRangeFromSensorQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<Empty> CreateReading(CreateReadingRequest request, ServerCallContext context)
        {
            var command = new CreateReadingCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<Empty> UpdateReading(UpdateReadingRequest request, ServerCallContext context)
        {
            var command = new UpdateReadingCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<Empty> DeleteReading(DeleteReadingRequest request, ServerCallContext context)
        {
            var command = new DeleteReadingCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }


        public override async Task<Empty> DeleteManyReadings(DeleteManyReadingsRequest request,
            ServerCallContext context)
        {
            var command = new DeleteManyReadingsCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<Empty> SaveReadingsFromCsv(
            IAsyncStreamReader<SaveReadingsFromCsvChunk> requestStream,
            ServerCallContext context)
        {
            var command = new SaveReadingsFromCsvCommand(requestStream);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override Task GetRangeFromSensorAsFile(GetRangeFromSensorAsFileRequest request,
            IServerStreamWriter<GetRangeFromSensorFileChunk> responseStream,
            ServerCallContext context)
        {
            return base.GetRangeFromSensorAsFile(request, responseStream, context);
        }

        public override Task<Stat1DBucketResponse> CalculateStat1DForSensor(CalculateStat1DForSensorRequest request,
            ServerCallContext context)
        {
            return base.CalculateStat1DForSensor(request, context);
        }
    }
}