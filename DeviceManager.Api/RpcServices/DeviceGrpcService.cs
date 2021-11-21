using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Api.RpcServices
{
    public class DeviceGrpcService : Core.Proto.DeviceGrpcService.DeviceGrpcServiceBase
    {
        private readonly ILogger<DeviceGrpcService> _logger;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;

        public DeviceGrpcService(ILogger<DeviceGrpcService> logger, IDeviceService deviceService, IMapper mapper)
        {
            _logger = logger;
            _deviceService = deviceService;
            _mapper = mapper;
        }

        public override async Task GetAllDevices(GetAllDevicesRequest request,
            IServerStreamWriter<DeviceResource> responseStream, ServerCallContext context)
        {
            // TODO: add validators

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

        public override async Task<DeviceResource> GetDeviceById(GetDeviceRequest request, ServerCallContext context)
        {
            // TODO: add validators

            var userId = Guid.Parse(request.UserId);
            var deviceName = request.Name;

            var device = (request.IncludeLocation, request.IncludeSensors) switch
            {
                (true, true) => await _deviceService.GetDeviceWithAll(userId, deviceName),
                (true, false) => await _deviceService.GetDeviceWithLocation(userId, deviceName),
                (false, true) => await _deviceService.GetDeviceWithSensors(userId, deviceName),
                _ => await _deviceService.GetDevice(userId, deviceName)
            };

            var deviceResource = _mapper.Map<Device, DeviceResource>(device);
            return await Task.FromResult(deviceResource);
        }

        public override async Task<GetDeviceApiKeyResponse> GetDeviceApiKey(GetDeviceApiKeyRequest request,
            ServerCallContext context)
        {
            // TODO: add validators
            var userId = Guid.Parse(request.UserId);
            var deviceName = request.Name;

            var device = await _deviceService.GetDevice(userId, deviceName);
            var response = new GetDeviceApiKeyResponse { ApiKey = device.ApiKey };

            return await Task.FromResult(response);
        }

        public override async Task<Empty> Ping(PingRequest request, ServerCallContext context)
        {
            // TODO: add validators
            var userId = Guid.Parse(request.UserId);
            var deviceName = request.Name;

            var device = await _deviceService.GetDevice(userId, deviceName);

            await _deviceService.UpdateDeviceLastSeen(device);

            return await Task.FromResult(new Empty());
        }

        public override async Task<DeviceResource> CreateDevice(CreateDeviceResource request, ServerCallContext context)
        {
            // TODO: add validators
            var newDevice = _mapper.Map<CreateDeviceResource, Device>(request);

            var sensors = request.Sensors
                .Select(s => _mapper.Map<SaveSensorRequest, Sensor>(s));

            var createdDevice = await _deviceService.CreateDevice(newDevice, sensors);

            return await Task.FromResult(_mapper.Map<Device, DeviceResource>(createdDevice));
        }

        public override async Task<DeviceResource> UpdateDevice(UpdateDeviceResource request, ServerCallContext context)
        {
            // TODO: add validators
            var userId = Guid.Parse(request.UserId);
            var deviceName = request.Name;
            var device = await _deviceService.GetDevice(userId, deviceName);

            await _deviceService
                .UpdateDevice(device, _mapper.Map<UpdateDeviceResource, Device>(request));

            return await Task.FromResult<DeviceResource>(_mapper.Map<Device, DeviceResource>(device));
        }

        public override Task<Empty> DeleteDevice(DeleteDeviceRequest request, ServerCallContext context)
        {
            /* TODO: Tutaj trochę skomplikowane bo trzeba nie tylko usunąć samo urządzenie ale również sensory
             TODO: i dane zebrane z sensorów w innym serwisie (bo nastąpi desynchronizacja danych AAAAAAAA) */  
            return base.DeleteDevice(request, context);
        }
    }
}