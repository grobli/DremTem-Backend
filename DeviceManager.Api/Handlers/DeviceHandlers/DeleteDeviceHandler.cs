using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Messages;
using DeviceManager.Core.Services;
using EasyNetQ;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Proto;
using Shared.Extensions;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class DeleteDeviceHandler : IRequestHandler<DeleteDeviceCommand, DeleteDeviceResponse>
    {
        private readonly IDeviceService _deviceService;
        private readonly IValidator<GenericDeleteRequest> _validator;
        private readonly IBus _bus;

        public DeleteDeviceHandler(IDeviceService deviceService, IValidator<GenericDeleteRequest> validator, IBus bus)
        {
            _deviceService = deviceService;
            _validator = validator;
            _bus = bus;
        }

        public async Task<DeleteDeviceResponse> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var device = await _deviceService.GetDeviceQuery(request.Body.Id, request.Body.UserId())
                .Include(d => d.Sensors).SingleOrDefaultAsync(cancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            var response = new DeleteDeviceResponse
                { DeletedDeviceId = device.Id, DeletedSensorIds = { device.Sensors.Select(s => s.Id) } };

            await _deviceService.DeleteDeviceAsync(device, cancellationToken);

            var message = new DeletedDeviceMessage(device.Id, response.DeletedSensorIds.ToList());
            await _bus.PubSub.PublishAsync(message, cancellationToken);

            return response;
        }
    }
}