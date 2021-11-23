using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using DeviceManager.Core.Services.DeviceTokenService;
using DeviceManager.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Api.RpcServices
{
    public class DeviceGrpcService : Core.Proto.DeviceGrpcService.DeviceGrpcServiceBase
    {
        private readonly ILogger<DeviceGrpcService> _logger;
        private readonly IDeviceService _deviceService;
        private readonly IDeviceTokenService _tokenService;
        private readonly IMapper _mapper;

        private readonly IValidator<CreateDeviceRequest> _createDeviceValidator;
        private readonly IValidator<GetAllDevicesRequest> _getAllDevicesValidator;
        private readonly IValidator<UpdateDeviceRequest> _updateDeviceValidator;
        private readonly IValidator<GetDeviceRequest> _getDeviceValidator;
        private readonly IValidator<GenerateTokenRequest> _generateTokenValidator;

        public DeviceGrpcService(
            ILogger<DeviceGrpcService> logger,
            IDeviceService deviceService,
            IDeviceTokenService tokenService,
            IMapper mapper,
            IValidator<CreateDeviceRequest> createDeviceValidator,
            IValidator<GetAllDevicesRequest> getAllDevicesValidator,
            IValidator<UpdateDeviceRequest> updateDeviceValidator,
            IValidator<GetDeviceRequest> getDeviceValidator,
            IValidator<GenerateTokenRequest> generateTokenValidator)
        {
            _logger = logger;
            _deviceService = deviceService;
            _tokenService = tokenService;
            _mapper = mapper;

            _createDeviceValidator = createDeviceValidator;
            _getAllDevicesValidator = getAllDevicesValidator;
            _updateDeviceValidator = updateDeviceValidator;
            _getDeviceValidator = getDeviceValidator;
            _generateTokenValidator = generateTokenValidator;
        }

        public override async Task GetAllDevices(GetAllDevicesRequest request,
            IServerStreamWriter<DeviceResource> responseStream, ServerCallContext context)
        {
            var validationResult = await _getAllDevicesValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            Guid? userId = string.IsNullOrWhiteSpace(request.UserId)
                ? null
                : Guid.Parse(request.UserId);

            var devices = (request.IncludeLocation, request.IncludeSensors) switch
            {
                (true, true) => await _deviceService.GetAllDevicesWithAll(userId),
                (true, false) => await _deviceService.GetAllDevicesWithLocation(userId),
                (false, true) => await _deviceService.GetAllDevicesWithSensors(userId),
                _ => await _deviceService.GetAllDevices(userId)
            };

            foreach (var device in devices)
            {
                await responseStream.WriteAsync(_mapper.Map<Device, DeviceResource>(device));
            }
        }

        public override async Task<DeviceResource> GetDevice(GetDeviceRequest request, ServerCallContext context)
        {
            var validationResult = await _getDeviceValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }


            var device = (request.IncludeLocation, request.IncludeSensors) switch
            {
                (true, true) => await _deviceService.GetDeviceWithAll(request.Id),
                (true, false) => await _deviceService.GetDeviceWithLocation(request.Id),
                (false, true) => await _deviceService.GetDeviceWithSensors(request.Id),
                _ => await _deviceService.GetDevice(request.Id)
            };

            Guid? userId = string.IsNullOrWhiteSpace(request.UserId) ? null : Guid.Parse(request.UserId);
            if (device is null || userId.HasValue && device.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Device with Id = {request.Id} not found"));
            }

            var deviceResource = _mapper.Map<Device, DeviceResource>(device);
            return await Task.FromResult(deviceResource);
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

            var createdDevice = await _deviceService.CreateDevice(newDevice, sensors);

            return await Task.FromResult(_mapper.Map<Device, DeviceResource>(createdDevice));
        }

        public override async Task<DeviceResource> UpdateDevice(UpdateDeviceRequest request, ServerCallContext context)
        {
            var validationResult = await _updateDeviceValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            Guid? userId = string.IsNullOrWhiteSpace(request.UserId) ? null : Guid.Parse(request.UserId);
            var device = await _deviceService.GetDevice(request.Id);
            if (device is null || userId.HasValue && device.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Device with Id = {request.Id} not found"));
            }

            await _deviceService
                .UpdateDevice(device, _mapper.Map<UpdateDeviceRequest, Device>(request));

            return await Task.FromResult(_mapper.Map<Device, DeviceResource>(device));
        }

        public override Task<Empty> DeleteDevice(DeleteDeviceRequest request, ServerCallContext context)
        {
            /* TODO: Tutaj trochę skomplikowane bo trzeba nie tylko usunąć samo urządzenie ale również sensory
             TODO: i dane zebrane z sensorów w innym serwisie (bo nastąpi desynchronizacja danych AAAAAAAA) */
            return base.DeleteDevice(request, context);
        }

        public override async Task<Empty> Ping(PingRequest request, ServerCallContext context)
        {
            var device = await _deviceService.GetDevice(request.Id);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Device with Id = {request.Id} not found"));
            }

            await _deviceService.UpdateDeviceLastSeen(device);

            return await Task.FromResult(new Empty());
        }

        public override async Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request,
            ServerCallContext context)
        {
            var validationResult = await _generateTokenValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            Guid? userId = string.IsNullOrWhiteSpace(request.UserId) ? null : Guid.Parse(request.UserId);
            var device = await _deviceService.GetDevice(request.Id);
            if (device is null || userId.HasValue && device.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Device with Id = {request.Id} not found"));
            }

            var response = _mapper.Map<Device, GenerateTokenResponse>(device);
            response.Token = await _tokenService.GenerateTokenAsync(device);

            return await Task.FromResult(response);
        }

        public override async Task<DeviceResource> GetDeviceByToken(GetDeviceByTokenRequest request,
            ServerCallContext context)
        {
            TokenContent token;

            try
            {
                token = await _tokenService.DecodeTokenAsync(request.Token);
            }
            catch (DecodeTokenException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }

            var device = await _deviceService.GetDevice(token.DeviceId);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"No device associated with this token"));
            }

            return await Task.FromResult(_mapper.Map<Device, DeviceResource>(device));
        }
    }
}