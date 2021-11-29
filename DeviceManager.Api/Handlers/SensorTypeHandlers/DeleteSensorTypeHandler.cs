using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Messages;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using EasyNetQ;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.SensorTypeHandlers
{
    public class DeleteSensorTypeHandler : IRequestHandler<DeleteSensorTypeCommand, DeleteSensorTypeResponse>
    {
        private readonly ISensorTypeService _typeService;
        private readonly IValidator<GenericDeleteRequest> _validator;
        private readonly IBus _bus;

        public DeleteSensorTypeHandler(ISensorTypeService typeService, IValidator<GenericDeleteRequest> validator,
            IBus bus)
        {
            _typeService = typeService;
            _validator = validator;
            _bus = bus;
        }

        public async Task<DeleteSensorTypeResponse> Handle(DeleteSensorTypeCommand request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorTypeQuery(request.Body.Id)
                .SingleOrDefaultAsync(cancellationToken);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            var response = new DeleteSensorTypeResponse { DeletedSensorTypeId = type.Id };
            await _typeService.DeleteSensorTypeAsync(type, cancellationToken);
            var message = new DeletedSensorTypeMessage(response.DeletedSensorTypeId);
            await _bus.PubSub.PublishAsync(message, cancellationToken);
            return response;
        }
    }
}