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

namespace DeviceManager.Api.Handlers.SensorHandlers
{
    public class CreateSensorHandler : IRequestHandler<CreateSensorCommand, SensorDto>
    {
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSensorRequest> _validator;
        private readonly IBus _bus;

        public CreateSensorHandler(ISensorService sensorService, IMapper mapper,
            IValidator<CreateSensorRequest> validator, IBus bus)
        {
            _sensorService = sensorService;
            _mapper = mapper;
            _validator = validator;
            _bus = bus;
        }

        public async Task<SensorDto> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newSensor = _mapper.Map<CreateSensorRequest, Sensor>(request.Body);

            try
            {
                var createdSensor = await _sensorService.CreateSensorAsync(newSensor, cancellationToken);
                var result = _mapper.Map<Sensor, SensorDto>(createdSensor);
                var message = new CreatedSensorMessage(result);
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