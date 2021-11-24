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
        private readonly IValidator<GetSensorTypeRequest> _getTypeValidator;
        private readonly IValidator<CreateSensorTypeRequest> _createTypeValidator;
        private readonly IValidator<UpdateSensorTypeRequest> _updateTypeValidator;
        private readonly IValidator<DeleteSensorTypeRequest> _deleteTypeValidator;

        public SensorTypeGrpcService(
            ILogger<SensorTypeGrpcService> logger,
            ISensorTypeService typeService,
            IMapper mapper,
            IValidator<CreateSensorTypeRequest> createTypeValidator,
            IValidator<GetSensorTypeRequest> getTypeValidator,
            IValidator<UpdateSensorTypeRequest> updateTypeValidator,
            IValidator<DeleteSensorTypeRequest> deleteTypeValidator)
        {
            _logger = logger;
            _typeService = typeService;
            _mapper = mapper;

            _createTypeValidator = createTypeValidator;
            _getTypeValidator = getTypeValidator;
            _updateTypeValidator = updateTypeValidator;
            _deleteTypeValidator = deleteTypeValidator;
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
            var validationResult = await _getTypeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var sensorType = await _typeService.GetSensorType(request.Id);

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(sensorType));
        }

        public override async Task<SensorTypeResource> CreateSensorType(CreateSensorTypeRequest request,
            ServerCallContext context)
        {
            var validationResult = await _createTypeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newType = _mapper.Map<CreateSensorTypeRequest, SensorType>(request);

            var createdType = await _typeService.CreateSensorType(newType);

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(createdType));
        }

        public override async Task<SensorTypeResource> UpdateSensorType(UpdateSensorTypeRequest request,
            ServerCallContext context)
        {
            var validationResult = await _updateTypeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorType(request.Id);
            await _typeService
                .UpdateSensorType(type, _mapper.Map<UpdateSensorTypeRequest, SensorType>(request));

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(type));
        }

        public override async Task<Empty> DeleteSensorType(DeleteSensorTypeRequest request, ServerCallContext context)
        {
            var validationResult = await _deleteTypeValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorType(request.Id);
            await _typeService.DeleteSensorType(type);

            return await Task.FromResult(new Empty());
        }
    }
}