using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Api.RpcServices
{
    public class SensorTypeGrpcService : Core.Proto.SensorTypeGrpcService.SensorTypeGrpcServiceBase
    {
        private readonly ILogger<SensorTypeGrpcService> _logger;
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;

        // validators
        private IValidator<SaveSensorTypeRequest> _saveTypeValidator;

        public SensorTypeGrpcService(
            ILogger<SensorTypeGrpcService> logger,
            ISensorTypeService typeService,
            IMapper mapper,
            IValidator<SaveSensorTypeRequest> saveTypeValidator)
        {
            _logger = logger;
            _typeService = typeService;
            _mapper = mapper;

            _saveTypeValidator = saveTypeValidator;
        }

        public override async Task GetAllSensorTypes(GetAllSensorTypesRequest request,
            IServerStreamWriter<SensorTypeResource> responseStream, ServerCallContext context)
        {
            var sensorTypes = await _typeService.GetAllSensorTypes();

            foreach (var type in sensorTypes)
            {
                await responseStream.WriteAsync(_mapper.Map<SensorType, SensorTypeResource>(type));
            }
        }

        public override async Task<SensorTypeResource> GetSensorType(GetSensorTypeRequest request,
            ServerCallContext context)
        {
            var sensorType = await _typeService.GetSensorType(request.Name);
            if (sensorType is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Sensor Type with Name = {request.Name} not found"));
            }

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(sensorType));
        }

        public override async Task<SensorTypeResource> CreateSensorType(SaveSensorTypeRequest request,
            ServerCallContext context)
        {
            var validationResult = await _saveTypeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var existingType = await _typeService.GetSensorType(request.Name);
            if (existingType is not null)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, $"Sensor Type - Name is already taken"));
            }

            var newType = _mapper.Map<SaveSensorTypeRequest, SensorType>(request);

            var createdType = await _typeService.CreateSensorType(newType);

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(createdType));
        }

        public override async Task<SensorTypeResource> UpdateSensorType(SaveSensorTypeRequest request,
            ServerCallContext context)
        {
            var validationResult = await _saveTypeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorType(request.Name);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Sensor Type with Name = \"{request.Name}\" not found"));
            }

            await _typeService
                .UpdateSensorType(type, _mapper.Map<SaveSensorTypeRequest, SensorType>(request));

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(type));
        }

        public override async Task<Empty> DeleteSensorType(DeleteSensorTypeRequest request, ServerCallContext context)
        {
            var type = await _typeService.GetSensorType(request.Name);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Sensor Type with Name = \"{request.Name}\" not found"));
            }

            await _typeService.DeleteSensorType(type);

            return await Task.FromResult(new Empty());
        }
    }
}