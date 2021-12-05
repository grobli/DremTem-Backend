using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Messages;
using EasyNetQ.AutoSubscribe;

namespace SensorData.Api.Consumers
{
    public class SensorMessageConsumer : IConsumeAsync<DeletedSensorMessage>
    {
        public async Task ConsumeAsync(DeletedSensorMessage message, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}