using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using SensorData.Api.Commands;
using SensorData.Core.Models;
using SensorData.Core.Services;
using Shared.Proto.SensorData;

namespace SensorData.Api.Handlers
{
    public class DeleteManyReadingsHandler : IRequestHandler<DeleteManyReadingsCommand, Empty>
    {
        private readonly IReadingService _readingService;
        private readonly IMapper _mapper;

        public DeleteManyReadingsHandler(IReadingService readingService, IMapper mapper)
        {
            _readingService = readingService;
            _mapper = mapper;
        }

        public async Task<Empty> Handle(DeleteManyReadingsCommand command, CancellationToken cancellationToken)
        {
            var readingKeys = command.Body.DeleteRequests
                .Select(r => _mapper.Map<DeleteReadingRequest, Reading>(r))
                .Select(r => new ReadingKey { Time = r.Time, SensorId = r.SensorId });

            var readingsToDelete = _readingService.GetAllReadingsQuery().Where(r =>
                readingKeys.Contains(new ReadingKey { Time = r.Time, SensorId = r.SensorId }));

            await _readingService.DeleteReadingsRangeAsync(readingsToDelete, cancellationToken);
            return new Empty();
        }

        
        private struct ReadingKey
        {
            public DateTime Time { get; set; }
            public int SensorId { get; set; }
        }
    }
}