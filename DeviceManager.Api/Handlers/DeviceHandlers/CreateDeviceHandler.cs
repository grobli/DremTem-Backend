using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Messages;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using EasyNetQ;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Proto;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class CreateDeviceHandler : IRequestHandler<CreateDeviceCommand, DeviceExtendedDto>
    {
        private readonly IDeviceService _deviceService;
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateDeviceRequest> _validator;
        private readonly IBus _bus;

        public CreateDeviceHandler(IDeviceService deviceService, ISensorService sensorService, IMapper mapper,
            IValidator<CreateDeviceRequest> validator, IBus bus)
        {
            _deviceService = deviceService;
            _sensorService = sensorService;
            _mapper = mapper;
            _validator = validator;
            _bus = bus;
        }

        public async Task<DeviceExtendedDto> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newDevice = _mapper.Map<CreateDeviceRequest, Device>(request.Body);
            var sensors = request.Body.Sensors
                .Select(s => _mapper.Map<CreateDeviceSensorResource, Sensor>(s))
                .ToList();
            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(newDevice, cancellationToken);
                foreach (var sensor in sensors) sensor.DeviceId = createdDevice.Id;
                await _sensorService.CreateSensorsRangeAsync(sensors, cancellationToken);

                var createdDeviceExtended = await _deviceService.GetDeviceQuery(createdDevice.Id)
                    .Include(d => d.Location)
                    .Include(d => d.Sensors)
                    .SingleOrDefaultAsync(cancellationToken);

                var result = _mapper.Map<Device, DeviceExtendedDto>(createdDeviceExtended);
                var message = new CreatedDeviceMessage(result);
                await _bus.PubSub.PublishAsync(message, cancellationToken);

                return result;
            }
            catch (ValidationException e)
            {
                if (_deviceService.GetDeviceQuery(newDevice.Id).Any())
                    await _deviceService.DeleteDeviceAsync(newDevice, cancellationToken);
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}