using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Messages;
using EasyNetQ.AutoSubscribe;

namespace SensorData.Api.Consumers
{
    public class DeviceMessageConsumer : IConsumeAsync<DeletedDeviceMessage>
    {
        public async Task ConsumeAsync(DeletedDeviceMessage message, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}