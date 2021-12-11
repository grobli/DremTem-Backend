using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using SensorData.Api.Commands;
using SensorData.Core.Services;

namespace SensorData.Api.Handlers
{
    public class DeleteManyReadingsHandler : IRequestHandler<DeleteManyReadingsCommand, Empty>
    {
        private readonly IReadingService _readingService;

        public DeleteManyReadingsHandler(IReadingService readingService)
        {
            _readingService = readingService;
        }

        public async Task<Empty> Handle(DeleteManyReadingsCommand command, CancellationToken cancellationToken)
        {
            var times = command.Body.Timestamps.Select(ts => ts.ToDateTime().ToUniversalTime());

            var readingsToDelete = _readingService.GetAllReadingsQuery()
                .Where(r => r.SensorId == command.Body.SensorId && times.Contains(r.Time));

            await _readingService.DeleteReadingsRangeAsync(readingsToDelete, cancellationToken);
            return new Empty();
        }
    }
}