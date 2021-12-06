using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Messages;
using EasyNetQ.AutoSubscribe;
using SensorData.Core.Services;

namespace SensorData.Api.Consumers
{
    public class DeviceMessageConsumer : IConsumeAsync<DeletedDeviceMessage>
    {
        private readonly IReadingService _readingService;

        public DeviceMessageConsumer(IReadingService readingService)
        {
            _readingService = readingService;
        }

        public async Task ConsumeAsync(DeletedDeviceMessage message, CancellationToken cancellationToken = default)
        {
            var readingsToDelete = _readingService.GetAllReadingsQuery()
                .Where(r => message.DeletedSensorIds.Contains(r.SensorId));

            await _readingService.DeleteReadingsRangeAsync(readingsToDelete, cancellationToken);
        }
    }
}