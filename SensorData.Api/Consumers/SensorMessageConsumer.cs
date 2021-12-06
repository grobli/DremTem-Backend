using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Messages;
using EasyNetQ.AutoSubscribe;
using SensorData.Core.Services;

namespace SensorData.Api.Consumers
{
    public class SensorMessageConsumer : IConsumeAsync<DeletedSensorMessage>
    {
        private readonly IReadingService _readingService;

        public SensorMessageConsumer(IReadingService readingService)
        {
            _readingService = readingService;
        }

        public async Task ConsumeAsync(DeletedSensorMessage message, CancellationToken cancellationToken = default)
        {
            var readingsToDelete = _readingService.GetAllReadingsQuery()
                .Where(r => message.DeletedSensorId == r.SensorId);

            await _readingService.DeleteReadingsRangeAsync(readingsToDelete, cancellationToken);
        }
    }
}