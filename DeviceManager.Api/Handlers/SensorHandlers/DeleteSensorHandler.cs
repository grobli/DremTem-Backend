using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Messages;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using EasyNetQ;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.SensorHandlers
{
    public class DeleteSensorHandler : IRequestHandler<DeleteSensorCommand, DeleteSensorResponse>
    {
        private readonly ISensorService _sensorService;
        private readonly IValidator<GenericDeleteRequest> _validator;
        private readonly IBus _bus;

        public DeleteSensorHandler(ISensorService sensorService, IValidator<GenericDeleteRequest> validator, IBus bus)
        {
            _sensorService = sensorService;
            _validator = validator;
            _bus = bus;
        }

        public async Task<DeleteSensorResponse> Handle(DeleteSensorCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var sensor = await _sensorService.GetSensorQuery(request.Body.Id, request.Body.UserId())
                .SingleOrDefaultAsync(cancellationToken);
            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            var response = new DeleteSensorResponse { DeletedSensorId = sensor.Id, ParentDeviceId = sensor.DeviceId };
            var message = new DeletedSensorMessage(response.DeletedSensorId, response.ParentDeviceId);
            await _bus.PubSub.PublishAsync(message, cancellationToken);

            await _sensorService.DeleteSensorAsync(sensor, cancellationToken);
            return response;
        }
    }
}