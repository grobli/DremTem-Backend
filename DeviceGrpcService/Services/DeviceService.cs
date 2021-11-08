using System;
using System.Threading.Tasks;
using AutoMapper;
using DeviceGrpcService.Data;
using DeviceGrpcService.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Device = DeviceGrpcService.Proto.Device;

namespace DeviceGrpcService.Services
{
    public class DeviceService : Device.DeviceBase
    {
        private readonly ILogger<DeviceService> _logger;
        private readonly DeviceContext _context;
        private readonly IMapper _mapper;

        public DeviceService(ILogger<DeviceService> logger, DeviceContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetAllDevices(Empty request, IServerStreamWriter<DeviceGrpcBaseModel> responseStream,
            ServerCallContext context)
        {
            foreach (var device in _context.Devices)
            {
                await responseStream.WriteAsync(_mapper.Map<DeviceGrpcBaseModel>(device));
            }
        }

        public override async Task GetAllDevicesNested(Empty request,
            IServerStreamWriter<DeviceGrpcNestedModel> responseStream, ServerCallContext context)
        {
            foreach (var device in _context.Devices
                .Include(d => d.Location))
            {
                await responseStream.WriteAsync(_mapper.Map<DeviceGrpcNestedModel>(device));
            }
        }

        public override async Task<DeviceGrpcBaseModel> GetDeviceById(DeviceByIdRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.ID);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.ID}\" does not exist"));
            }

            return await Task.FromResult(_mapper.Map<DeviceGrpcBaseModel>(device));
        }

        public override async Task<DeviceGrpcNestedModel> GetDeviceByIdNested(DeviceByIdRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.ID, true);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.ID}\" does not exist"));
            }

            return await Task.FromResult(_mapper.Map<DeviceGrpcNestedModel>(device));
        }

        private async Task<Models.Device> GetDeviceFromDatabaseAsync(string id,
            bool includeNavigations = false)
        {
            if (!Guid.TryParse(id, out var requestId)) return null;
            if (includeNavigations)
            {
                return await _context.Devices
                    .Include(d => d.Location)
                    .FirstOrDefaultAsync(d => d.ID == requestId);
            }

            return await _context.Devices.FirstOrDefaultAsync(d => d.ID == requestId);
        }

        public override async Task<DeviceGrpcBaseModel> CreateDevice(DeviceCreateRequest request,
            ServerCallContext context)
        {
            var name = GetNameValueFromRequest(request);
            var locationId = GetLocationIdValueFromRequest(request);

            if (locationId != null && !await CheckLocationExistsAsync((int)locationId))
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified location with ID={locationId} does not exist"));
            }

            var device = new Models.Device
                { Name = name, Online = request.Online, LocationID = locationId };

            try
            {
                _context.Devices.Add(device);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Unable to save changes");
                throw new RpcException(new Status(StatusCode.Internal,
                    "Unable to save changes." +
                    "Try again, and if the problem persists see your system administrator."));
            }

            return await Task.FromResult(_mapper.Map<DeviceGrpcBaseModel>(device));
        }

        private async Task<bool> CheckLocationExistsAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            return location != null;
        }

        private static string GetNameValueFromRequest(DeviceCreateRequest request)
        {
            return request.NameCase switch
            {
                DeviceCreateRequest.NameOneofCase.None => null,
                DeviceCreateRequest.NameOneofCase.NameValue => request.NameValue,
                _ => throw new ArgumentException("Invalid message - NameCase")
            };
        }

        private static (string value, bool clearFlag) GetNameValueFromRequest(DeviceUpdateRequest request)
        {
            return request.NameCase switch
            {
                DeviceUpdateRequest.NameOneofCase.None => (null, false),
                DeviceUpdateRequest.NameOneofCase.NameValue => (request.NameValue, false),
                DeviceUpdateRequest.NameOneofCase.NameNullified => (null, request.NameNullified),
                _ => throw new ArgumentException("Invalid message - NameCase")
            };
        }

        private static bool? GetOnlineValueFromRequest(DeviceUpdateRequest request)
        {
            return request.OnlineCase switch
            {
                DeviceUpdateRequest.OnlineOneofCase.None => null,
                DeviceUpdateRequest.OnlineOneofCase.OnlineValue => request.OnlineValue,
                _ => throw new ArgumentException("Invalid message - OnlineValue")
            };
        }

        private static int? GetLocationIdValueFromRequest(DeviceCreateRequest request)
        {
            return request.LocationIDCase switch
            {
                DeviceCreateRequest.LocationIDOneofCase.None => null,
                DeviceCreateRequest.LocationIDOneofCase.LocationIdValue => request.LocationIdValue,
                _ => throw new ArgumentException("Invalid message - LocationIDCase")
            };
        }

        private static (int? value, bool clearFlag) GetLocationIdValueFromRequest(DeviceUpdateRequest request)
        {
            return request.LocationIDCase switch
            {
                DeviceUpdateRequest.LocationIDOneofCase.None => (null, false),
                DeviceUpdateRequest.LocationIDOneofCase.LocationIdValue => (request.LocationIdValue, false),
                DeviceUpdateRequest.LocationIDOneofCase.LocationNullified => (null, request.LocationNullified),
                _ => throw new ArgumentException("Invalid message - LocationIDCase")
            };
        }

        public override async Task<DeviceGrpcBaseModel> UpdateDeviceByID(DeviceUpdateRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.ID, true);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.ID}\" does not exist"));
            }

            var (name, nameClearFlag) = GetNameValueFromRequest(request);
            var (locationId, locationClearFlag) = GetLocationIdValueFromRequest(request);
            var online = GetOnlineValueFromRequest(request);

            device.Name = nameClearFlag ? null : name ?? device.Name;
            device.LocationID = locationClearFlag ? null : locationId ?? device.LocationID;
            device.Online = online ?? device.Online;

            try
            {
                _context.Devices.Update(device);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Unable to save changes");
                throw new RpcException(new Status(StatusCode.Internal,
                    "Unable to save changes." +
                    "Try again, and if the problem persists see your system administrator."));
            }

            return await Task.FromResult(_mapper.Map<DeviceGrpcBaseModel>(device));
        }

        public override async Task<Empty> DeleteDeviceByID(DeviceByIdRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.ID, true);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.ID}\" does not exist"));
            }

            try
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Unable to save changes");
                throw new RpcException(new Status(StatusCode.Internal,
                    "Unable to save changes." +
                    "Try again, and if the problem persists see your system administrator."));
            }

            return await Task.FromResult(new Empty());
        }
    }
}