using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Proto;
using EasyNetQ.AutoSubscribe;
using MediatR;
using UserIdentity.Core.Messages;

namespace DeviceManager.Api.Consumers
{
    public class UserMessageConsumer : IConsumeAsync<DeletedUserMessage>
    {
        private readonly IMediator _mediator;

        public UserMessageConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(DeletedUserMessage message,
            CancellationToken cancellationToken = default)
        {
            await DeleteUsersDevicesAndSensors(message, cancellationToken);
            await DeleteUsersLocations(message, cancellationToken);
        }

        private async Task DeleteUsersDevicesAndSensors(DeletedUserMessage message,
            CancellationToken cancellationToken = default)
        {
            var devicesQuery = new GetAllDevicesQuery(new GenericGetManyRequest
            {
                Parameters = new GetRequestParameters { UserId = message.DeletedUserId.ToString() },
                PageSize = int.MaxValue,
                PageNumber = 1
            });
            var devices = (await _mediator.Send(devicesQuery, cancellationToken)).Devices;
            foreach (var device in devices)
            {
                var command = new DeleteDeviceCommand(new GenericDeleteRequest
                    { Id = device.Id, UserId = message.DeletedUserId.ToString() });
                await _mediator.Send(command, cancellationToken);
            }
        }

        private async Task DeleteUsersLocations(DeletedUserMessage message,
            CancellationToken cancellationToken = default)
        {
            var locationsQuery = new GetAllLocationsQuery(new GenericGetManyRequest
            {
                Parameters = new GetRequestParameters { UserId = message.DeletedUserId.ToString() },
                PageSize = int.MaxValue,
                PageNumber = 1
            });
            var locations = (await _mediator.Send(locationsQuery, cancellationToken)).Locations;
            foreach (var location in locations)
            {
                var command = new DeleteLocationCommand(new GenericDeleteRequest
                    { Id = location.Id, UserId = message.DeletedUserId.ToString() });
                await _mediator.Send(command, cancellationToken);
            }
        }
    }
}