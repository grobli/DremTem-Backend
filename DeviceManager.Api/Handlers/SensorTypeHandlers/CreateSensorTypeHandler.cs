using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Messages;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using EasyNetQ;
using FluentValidation;
using Grpc.Core;
using MediatR;

namespace DeviceManager.Api.Handlers.SensorTypeHandlers
{
    public class CreateSensorTypeHandler : IRequestHandler<CreateSensorTypeCommand, SensorTypeDto>
    {
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSensorTypeRequest> _validator;
        private readonly IBus _bus;

        public CreateSensorTypeHandler(ISensorTypeService typeService, IMapper mapper,
            IValidator<CreateSensorTypeRequest> validator, IBus bus)
        {
            _typeService = typeService;
            _mapper = mapper;
            _validator = validator;
            _bus = bus;
        }

        public async Task<SensorTypeDto> Handle(CreateSensorTypeCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newType = _mapper.Map<CreateSensorTypeRequest, SensorType>(request.Body);

            try
            {
                var createdType = await _typeService.CreateSensorTypeAsync(newType, cancellationToken);
                var result = _mapper.Map<SensorType, SensorTypeDto>(createdType);
                var message = new CreatedSensorTypeMessage(result);
                await _bus.PubSub.PublishAsync(message, cancellationToken);
                return result;
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}