using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Api.RpcServices
{
    public class SensorGrpcService : Core.Proto.SensorGrpcService.SensorGrpcServiceBase
    {
        private readonly ILogger<SensorGrpcService> _logger;
        private readonly ISensorService _sensorService;
        private readonly IMapper _mapper;

        // validators
        private readonly IValidator<GetAllSensorsRequest> _getAllSensorsValidator;
        private readonly IValidator<SaveSensorRequest> _saveSensorValidator;

        public SensorGrpcService(
            ILogger<SensorGrpcService> logger,
            ISensorService sensorService,
            IMapper mapper,
            IValidator<GetAllSensorsRequest> getAllSensorsValidator,
            IValidator<SaveSensorRequest> saveSensorValidator)
        {
            _logger = logger;
            _sensorService = sensorService;
            _mapper = mapper;

            _getAllSensorsValidator = getAllSensorsValidator;
            _saveSensorValidator = saveSensorValidator;
        }

        public override async Task GetAllSensors(GetAllSensorsRequest request,
            IServerStreamWriter<SensorResource> responseStream, ServerCallContext context)
        {
            var validationResult = await _getAllSensorsValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            Guid? userId = string.IsNullOrWhiteSpace(request.UserId)
                ? null
                : Guid.Parse(request.UserId);

            var sensors = request.IncludeType
                ? await _sensorService.GetAllSensorsWithType(userId)
                : await _sensorService.GetAllSensors(userId);

            foreach (var sensor in sensors)
            {
                await responseStream.WriteAsync(_mapper.Map<Sensor, SensorResource>(sensor));
            }
        }

        public override async Task<SensorResource> GetSensor(GetSensorRequest request, ServerCallContext context)
        {
            var sensor = request.IncludeType
                ? await _sensorService.GetSensorWithType(request.Id)
                : await _sensorService.GetSensor(request.Id);

            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Sensor with Id = {request.Id} not found"));
            }

            return await Task.FromResult(_mapper.Map<Sensor, SensorResource>(sensor));
        }

        public override async Task<SensorResource> AddSensor(SaveSensorRequest request, ServerCallContext context)
        {
            var validationResult = await _saveSensorValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newSensor = _mapper.Map<SaveSensorRequest, Sensor>(request);

            var createdSensor = await _sensorService.CreateSensor(newSensor);

            return await Task.FromResult(_mapper.Map<Sensor, SensorResource>(createdSensor));
        }

        public override async Task<SensorResource> UpdateSensor(SaveSensorRequest request, ServerCallContext context)
        {
            var validationResult = await _saveSensorValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            if (request.Id is null)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Missing sensor Id"));
            }

            var sensor = await _sensorService.GetSensor(request.Id.Value);
            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Sensor with Id = {request.Id.Value} not found"));
            }

            await _sensorService
                .UpdateSensor(sensor, _mapper.Map<SaveSensorRequest, Sensor>(request));

            return await Task.FromResult(_mapper.Map<Sensor, SensorResource>(sensor));
        }

        public override Task<SensorResource> DeleteSensor(DeleteSensorRequest request, ServerCallContext context)
        {
            /* TODO: Tutaj trochę skomplikowane bo trzeba nie tylko usunąć sam sensor ale również
          TODO: i dane zebrane z tego sensora w innym serwisie (bo nastąpi desynchronizacja danych AAAAAAAA) */
            
            return base.DeleteSensor(request, context);
        }
    }
}