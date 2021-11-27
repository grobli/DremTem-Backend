using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DeviceManager.Api.RpcServices
{
    public class SensorGrpcService : Core.Proto.SensorGrpcService.SensorGrpcServiceBase
    {
        private readonly ILogger<SensorGrpcService> _logger;
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;

        // validators
        private readonly IValidator<GenericGetManyRequest> _getAllValidator;
        private readonly IValidator<GenericGetRequest> _getValidator;
        private readonly IValidator<CreateSensorRequest> _createValidator;
        private readonly IValidator<UpdateSensorRequest> _updateValidator;

        public SensorGrpcService(
            ILogger<SensorGrpcService> logger,
            ISensorService sensorService,
            IMapper mapper,
            IValidator<GenericGetManyRequest> getAllValidator,
            IValidator<CreateSensorRequest> saveValidator,
            IValidator<GenericGetRequest> getValidator,
            IValidator<UpdateSensorRequest> updateValidator)
        {
            _logger = logger;
            _sensorService = sensorService;
            _mapper = mapper;

            _getAllValidator = getAllValidator;
            _createValidator = saveValidator;
            _getValidator = getValidator;
            _updateValidator = updateValidator;
        }

        public override async Task<GetAllSensorsResponse> GetAllSensors(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getAllValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = request.Parameters?.UserId() ?? Guid.Empty;
            var sensors = _sensorService.GetAllSensors(userId);
            if (request.Parameters != null &&
                request.Parameters.IncludeFieldsSet(Entity.SensorType).Contains(Entity.SensorType))
            {
                sensors = sensors.Include(s => s.Type);
            }

            var pagedList = await PagedList<Sensor>.ToPagedListAsync(sensors, request.PageNumber, request.PageSize,
                context.CancellationToken);

            var response = new GetAllSensorsResponse
            {
                Sensors = { pagedList.Select(s => _mapper.Map<Sensor, SensorResource>(s)) },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return await Task.FromResult(response);
        }

        public override async Task<SensorResource> GetSensor(GenericGetRequest request, ServerCallContext context)
        {
            var validationResult = await _getValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = request.Parameters?.UserId() ?? Guid.Empty;
            var sensorQuery = _sensorService.GetSensor(request.Id, userId);
            if (request.Parameters != null &&
                request.Parameters.IncludeFieldsSet(Entity.SensorType).Contains(Entity.SensorType))
            {
                sensorQuery = sensorQuery.Include(l => l.Type);
            }

            var sensor = await sensorQuery.SingleOrDefaultAsync(context.CancellationToken);
            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            return await Task.FromResult(_mapper.Map<Sensor, SensorResource>(sensor));
        }

        public override async Task<SensorResource> AddSensor(CreateSensorRequest request, ServerCallContext context)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newSensor = _mapper.Map<CreateSensorRequest, Sensor>(request);
            var createdSensor = await _sensorService.CreateSensorAsync(newSensor);

            return await Task.FromResult(_mapper.Map<Sensor, SensorResource>(createdSensor));
        }

        public override async Task<SensorResource> UpdateSensor(UpdateSensorRequest request, ServerCallContext context)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var sensor = await _sensorService.GetSensor(request.Id, request.UserId())
                .SingleOrDefaultAsync(context.CancellationToken);
            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _sensorService
                .UpdateSensorAsync(sensor, _mapper.Map<UpdateSensorRequest, Sensor>(request));

            return await Task.FromResult(_mapper.Map<Sensor, SensorResource>(sensor));
        }

        public override Task<SensorResource> DeleteSensor(GenericDeleteRequest request, ServerCallContext context)
        {
            /* TODO: Tutaj trochę skomplikowane bo trzeba nie tylko usunąć sam sensor ale również
          TODO: i dane zebrane z tego sensora w innym serwisie (bo nastąpi desynchronizacja danych AAAAAAAA) */

            return base.DeleteSensor(request, context);
        }
    }
}