using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DeviceManager.Api.RpcServices
{
    public class SensorTypeGrpcService : Core.Proto.SensorTypeGrpcService.SensorTypeGrpcServiceBase
    {
        private readonly ILogger<SensorTypeGrpcService> _logger;
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;

        // validators
        private readonly IValidator<GenericGetManyRequest> _getAllValidator;
        private readonly IValidator<GenericGetRequest> _getValidator;
        private readonly IValidator<CreateSensorTypeRequest> _createValidator;
        private readonly IValidator<UpdateSensorTypeRequest> _updateValidator;
        private readonly IValidator<GenericDeleteRequest> _deleteValidator;

        public SensorTypeGrpcService(
            ILogger<SensorTypeGrpcService> logger,
            ISensorTypeService typeService,
            IMapper mapper,
            IValidator<CreateSensorTypeRequest> createValidator,
            IValidator<GenericGetManyRequest> getAllValidator,
            IValidator<GenericGetRequest> getValidator,
            IValidator<UpdateSensorTypeRequest> updateValidator,
            IValidator<GenericDeleteRequest> deleteValidator)
        {
            _logger = logger;
            _typeService = typeService;
            _mapper = mapper;

            _createValidator = createValidator;
            _getValidator = getValidator;
            _getAllValidator = getAllValidator;
            _updateValidator = updateValidator;
            _deleteValidator = deleteValidator;
        }

        public override async Task<GetAllSensorTypesResponse> GetAllSensorTypes(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getAllValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var types = _typeService.GetAllSensorTypesQuery();
            var pagedList = await PagedList<SensorType>.ToPagedListAsync(types, request.PageNumber, request.PageSize,
                context.CancellationToken);

            var response = new GetAllSensorTypesResponse()
            {
                SensorTypes =
                {
                    pagedList.Select(t => _mapper.Map<SensorType, SensorTypeResource>(t))
                },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };
            return await Task.FromResult(response);
        }

        public override async Task<SensorTypeResource> GetSensorType(GenericGetRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorTypeQuery(request.Id).SingleOrDefaultAsync(context.CancellationToken);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(type));
        }

        public override async Task<SensorTypeResource> CreateSensorType(CreateSensorTypeRequest request,
            ServerCallContext context)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newType = _mapper.Map<CreateSensorTypeRequest, SensorType>(request);

            var createdType = await _typeService.CreateSensorTypeAsync(newType);

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(createdType));
        }

        public override async Task<SensorTypeResource> UpdateSensorType(UpdateSensorTypeRequest request,
            ServerCallContext context)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorTypeQuery(request.Id).SingleOrDefaultAsync(context.CancellationToken);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _typeService
                .UpdateSensorTypeAsync(type, _mapper.Map<UpdateSensorTypeRequest, SensorType>(request));

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeResource>(type));
        }

        public override async Task<Empty> DeleteSensorType(GenericDeleteRequest request, ServerCallContext context)
        {
            var validationResult = await _deleteValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorTypeQuery(request.Id).SingleOrDefaultAsync(context.CancellationToken);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _typeService.DeleteSensorTypeAsync(type);

            return await Task.FromResult(new Empty());
        }
    }
}