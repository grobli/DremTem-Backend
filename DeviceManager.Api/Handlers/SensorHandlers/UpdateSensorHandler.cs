using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeviceManager.Core.Messages;
using EasyNetQ;
using Shared.Extensions;
using Shared.Proto;
using Shared.Proto.Sensor;

namespace DeviceManager.Api.Handlers.SensorHandlers
{
    public class UpdateSensorHandler : IRequestHandler<UpdateSensorCommand, SensorDto>
    {
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateSensorRequest> _validator;
        private readonly IBus _bus;

        public UpdateSensorHandler(ISensorService sensorService, IMapper mapper,
            IValidator<UpdateSensorRequest> validator, IBus bus)
        {
            _sensorService = sensorService;
            _mapper = mapper;
            _validator = validator;
            _bus = bus;
        }

        public async Task<SensorDto> Handle(UpdateSensorCommand request, CancellationToken cancellationToken)
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

            try
            {
                await _sensorService
                    .UpdateSensorAsync(sensor, _mapper.Map<UpdateSensorRequest, Sensor>(request.Body),
                        cancellationToken);

                var result = _mapper.Map<Sensor, SensorDto>(sensor);
                var message = new UpdatedSensorMessage(result);
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