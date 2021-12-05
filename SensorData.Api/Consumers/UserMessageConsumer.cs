using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using UserIdentity.Core.Messages;

namespace SensorData.Api.Consumers
{
    public class UserMessageConsumer : IConsumeAsync<DeletedUserMessage>
    {
        private readonly IMediator _mediator;

        public UserMessageConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(DeletedUserMessage message,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}