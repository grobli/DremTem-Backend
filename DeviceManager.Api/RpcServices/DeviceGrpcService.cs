using System;
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
    public class DeviceGrpcService : Core.Proto.DeviceGrpcService.DeviceGrpcServiceBase
    {
        private readonly ILogger<DeviceGrpcService> _logger;
        private readonly IDeviceService _deviceService;
        private readonly IDeviceTokenService _tokenService;
        private readonly IMapper _mapper;

        private readonly IValidator<CreateDeviceRequest> _createDeviceValidator;
        private readonly IValidator<GenericGetManyRequest> _getManyValidator;
        private readonly IValidator<UpdateDeviceRequest> _updateDeviceValidator;
        private readonly IValidator<GenericGetRequest> _getValidator;
        private readonly IValidator<GenerateTokenRequest> _generateTokenValidator;

        public DeviceGrpcService(
            ILogger<DeviceGrpcService> logger,
            IDeviceService deviceService,
            IDeviceTokenService tokenService,
            IMapper mapper,
            IValidator<CreateDeviceRequest> createDeviceValidator,
            IValidator<GenericGetManyRequest> getManyValidator,
            IValidator<UpdateDeviceRequest> updateDeviceValidator,
            IValidator<GenericGetRequest> getValidator,
            IValidator<GenerateTokenRequest> generateTokenValidator)
        {
            _logger = logger;
            _deviceService = deviceService;
            _tokenService = tokenService;
            _mapper = mapper;

            _createDeviceValidator = createDeviceValidator;
            _getManyValidator = getManyValidator;
            _updateDeviceValidator = updateDeviceValidator;
            _getValidator = getValidator;
            _generateTokenValidator = generateTokenValidator;
        }

        public override async Task<GetAllDevicesResponse> GetAllDevices(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getManyValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = request.Parameters?.UserId() ?? Guid.Empty;
            var devices = _deviceService.GetAllDevices(userId);
            if (request.Parameters != null)
                devices = request.Parameters.IncludeFieldsSet(Entity.Location, Entity.Sensor)
                    .Aggregate(devices, (current, field) => field switch
                    {
                        Entity.Location => current.Include(d => d.Location),
                        Entity.Sensor => current.Include(d => d.Sensors),
                        _ => current
                    });

            var pagedList = await PagedList<Device>.ToPagedListAsync(devices, request.PageNumber,
                request.PageSize, context.CancellationToken);

            var response = new GetAllDevicesResponse
            {
                Devices = { pagedList.Select(d => _mapper.Map<Device, DeviceResourceExtended>(d)) },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return await Task.FromResult(response);
        }

        public override async Task<DeviceResourceExtended> GetDevice(GenericGetRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = request.Parameters?.UserId() ?? Guid.Empty;
            var deviceQuery = _deviceService.GetDevice(request.Id, userId);
            if (request.Parameters != null)
                deviceQuery = request.Parameters.IncludeFieldsSet(Entity.Location, Entity.Sensor)
                    .Aggregate(deviceQuery, (current, field) => field switch
                    {
                        Entity.Location => current.Include(d => d.Location),
                        Entity.Sensor => current.Include(d => d.Sensors),
                        _ => current
                    });

            var device = await deviceQuery.SingleOrDefaultAsync(context.CancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            return await Task.FromResult(_mapper.Map<Device, DeviceResourceExtended>(device));
        }

        public override async Task<DeviceResource> CreateDevice(CreateDeviceRequest request, ServerCallContext context)
        {
            var validationResult = await _createDeviceValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newDevice = _mapper.Map<CreateDeviceRequest, Device>(request);
            var sensors = request.Sensors
                .Select(s => _mapper.Map<CreateDeviceSensorResource, Sensor>(s));

            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(newDevice, sensors);
                return await Task.FromResult(_mapper.Map<Device, DeviceResource>(createdDevice));
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }

        public override async Task<DeviceResource> UpdateDevice(UpdateDeviceRequest request, ServerCallContext context)
        {
            var validationResult = await _updateDeviceValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var device = await _deviceService.GetDevice(request.Id, request.UserId()).SingleOrDefaultAsync();
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            try
            {
                await _deviceService
                    .UpdateDeviceAsync(device, _mapper.Map<UpdateDeviceRequest, Device>(request));
                return await Task.FromResult(_mapper.Map<Device, DeviceResource>(device));
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }

        public override Task<Empty> DeleteDevice(GenericDeleteRequest request, ServerCallContext context)
        {
            /* TODO: Tutaj trochę skomplikowane bo trzeba nie tylko usunąć samo urządzenie ale również sensory
             TODO: i dane zebrane z sensorów w innym serwisie (bo nastąpi desynchronizacja danych AAAAAAAA) */
            return base.DeleteDevice(request, context);
        }

        public override async Task<Empty> Ping(PingRequest request, ServerCallContext context)
        {
            var device = await _deviceService.GetDevice(request.Id).SingleOrDefaultAsync(context.CancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _deviceService.UpdateDeviceLastSeenAsync(device);
            return await Task.FromResult(new Empty());
        }

        public override async Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request,
            ServerCallContext context)
        {
            var validationResult = await _generateTokenValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var device = await _deviceService.GetDevice(request.Id, request.UserId())
                .SingleOrDefaultAsync(context.CancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            return await Task.FromResult(new GenerateTokenResponse
            {
                Id = device.Id,
                Token = await _tokenService.GenerateTokenAsync(device, context.CancellationToken)
            });
        }
    }
}