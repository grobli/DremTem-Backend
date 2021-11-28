using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
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

        public CreateSensorHandler(ISensorService sensorService, IMapper mapper,
            IValidator<CreateSensorRequest> validator)
        {
            _sensorService = sensorService;
            _mapper = mapper;
            _validator = validator;
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
            var createdSensor = await _sensorService.CreateSensorAsync(newSensor, cancellationToken);

            return _mapper.Map<Sensor, SensorDto>(createdSensor);
        }
    }
}