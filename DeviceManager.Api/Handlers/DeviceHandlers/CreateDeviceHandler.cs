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

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class CreateDeviceHandler : IRequestHandler<CreateDeviceCommand, DeviceDto>
    {
        private readonly IDeviceService _deviceService;
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateDeviceRequest> _validator;

        public CreateDeviceHandler(IDeviceService deviceService, ISensorService sensorService, IMapper mapper,
            IValidator<CreateDeviceRequest> validator)
        {
            _deviceService = deviceService;
            _sensorService = sensorService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<DeviceDto> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newDevice = _mapper.Map<CreateDeviceRequest, Core.Models.Device>(request.Body);
            var sensors = request.Body.Sensors
                .Select(s => _mapper.Map<CreateDeviceSensorResource, Sensor>(s))
                .ToList();
            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(newDevice, cancellationToken);
                foreach (var sensor in sensors) sensor.DeviceId = createdDevice.Id;
                await _sensorService.CreateSensorsRangeAsync(sensors, cancellationToken);

                return _mapper.Map<Core.Models.Device, DeviceDto>(createdDevice);
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}