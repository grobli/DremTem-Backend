using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using SensorData.Api.Commands;
using SensorData.Core.Models;
using SensorData.Core.Services;
using Shared.Proto;

namespace SensorData.Api.Handlers
{
    public class DeleteReadingHandler : IRequestHandler<DeleteReadingCommand, Empty>
    {
        private readonly IReadingService _readingService;
        private readonly IMapper _mapper;

        public DeleteReadingHandler(IReadingService readingService, IMapper mapper)
        {
            _readingService = readingService;
            _mapper = mapper;
        }

        public async Task<Empty> Handle(DeleteReadingCommand command, CancellationToken cancellationToken)
        {
            var readingKey = _mapper.Map<DeleteReadingRequest, Reading>(command.Body);
            var readingToDelete = await _readingService.GetReadingByTimestampAsync(readingKey.Time,
                readingKey.SensorId, cancellationToken);
            if (readingToDelete is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Reading not found"));
            }

            await _readingService.DeleteReadingAsync(readingToDelete, cancellationToken);
            return new Empty();
        }
    }
}