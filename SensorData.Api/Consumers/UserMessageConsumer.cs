using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Options;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using UserIdentity.Core.Messages;

namespace SensorData.Api.Consumers
{
    public class UserMessageConsumer : IConsumeAsync<DeletedUserMessage>
    {
        private readonly IReadingService _readingService;
        private readonly UserSettings _userSettings;

        public UserMessageConsumer(IReadingService readingService, IOptions<UserSettings> userSettings)
        {
            _readingService = readingService;
            _userSettings = userSettings.Value;
        }

        public async Task ConsumeAsync(DeletedUserMessage message, CancellationToken cancellationToken = default)
        {
            // delete everything if user was deleted from the system
            if (_userSettings.Id == message.DeletedUserId)
            {
                await _readingService.DeleteReadingsRangeAsync(_readingService.GetAllReadingsQuery(),
                    cancellationToken);
            }
        }
    }
}