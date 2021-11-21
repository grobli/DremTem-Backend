using System;
using System.Linq;
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

        public override async Task GetAllDevices(GetAllDevicesRequest request,
            IServerStreamWriter<DeviceResource> responseStream,
            ServerCallContext context)
        {
            foreach (var device in _context.Devices.Where(d => d.OwnerId.ToString() == request.OwnerId))
            {
                await responseStream.WriteAsync(_mapper.Map<DeviceResource>(device));
            }
        }

        public override async Task GetAllDevicesNested(GetAllDevicesRequest request,
            IServerStreamWriter<DeviceGrpcNestedModel> responseStream, ServerCallContext context)
        {
            foreach (var device in _context.Devices
                .Include(d => d.Location)
                .Where(d => d.OwnerId.ToString() == request.OwnerId))
            {
                await responseStream.WriteAsync(_mapper.Map<DeviceGrpcNestedModel>(device));
            }
        }

        public override async Task<DeviceResource> GetDeviceById(DeviceByIdRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.Id, request.OwnerId);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.Id}\" does not exist"));
            }

            return await Task.FromResult(_mapper.Map<DeviceResource>(device));
        }

        public override async Task<DeviceGrpcNestedModel> GetDeviceByIdNested(DeviceByIdRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.Id, request.OwnerId, true);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.Id}\" does not exist"));
            }

            return await Task.FromResult(_mapper.Map<DeviceGrpcNestedModel>(device));
        }

        public override async Task<DeviceResource> CreateDevice(DeviceCreateRequest request,
            ServerCallContext context)
        {
            if (request.LocationId is not null && !await CheckLocationExistsAsync((int)request.LocationId))
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified location with ID={request.LocationId} does not exist"));
            }

            var device = new Models.Device
                { Name = request.Name, Online = request.Online, LocationId = request.LocationId };

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

            return await Task.FromResult(_mapper.Map<DeviceResource>(device));
        }

        private async Task<bool> CheckLocationExistsAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            return location != null;
        }

        private static (string value, bool clearFlag) GetNameValueFromRequest(DeviceUpdateRequest request) =>
            request.NameCase switch
            {
                DeviceUpdateRequest.NameOneofCase.None => (null, false),
                DeviceUpdateRequest.NameOneofCase.NameValue => (request.NameValue, false),
                DeviceUpdateRequest.NameOneofCase.NameNullified => (null, request.NameNullified),
                _ => throw new ArgumentException("Invalid message - NameCase")
            };


        private static bool? GetOnlineValueFromRequest(DeviceUpdateRequest request) =>
            request.OnlineCase switch
            {
                DeviceUpdateRequest.OnlineOneofCase.None => null,
                DeviceUpdateRequest.OnlineOneofCase.OnlineValue => request.OnlineValue,
                _ => throw new ArgumentException("Invalid message - OnlineValue")
            };


        private static (int? value, bool clearFlag) GetLocationIdValueFromRequest(DeviceUpdateRequest request) =>
            request.LocationIDCase switch
            {
                DeviceUpdateRequest.LocationIDOneofCase.None => (null, false),
                DeviceUpdateRequest.LocationIDOneofCase.LocationIdValue => (request.LocationIdValue, false),
                DeviceUpdateRequest.LocationIDOneofCase.LocationNullified => (null, request.LocationNullified),
                _ => throw new ArgumentException("Invalid message - LocationIDCase")
            };


        public override async Task<DeviceResource> UpdateDeviceByID(DeviceUpdateRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.ID, request.OwnerId, true);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.ID}\" does not exist"));
            }

            var (name, nameClearFlag) = GetNameValueFromRequest(request);
            var (locationId, locationClearFlag) = GetLocationIdValueFromRequest(request);
            var online = GetOnlineValueFromRequest(request);

            device.Name = nameClearFlag ? null : name ?? device.Name;
            device.LocationId = locationClearFlag ? null : locationId ?? device.LocationId;
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

            return await Task.FromResult(_mapper.Map<DeviceResource>(device));
        }

        public override async Task<Empty> DeleteDeviceByID(DeviceByIdRequest request,
            ServerCallContext context)
        {
            var device = await GetDeviceFromDatabaseAsync(request.Id, request.OwnerId, true);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified device with ID=\"{request.Id}\" does not exist"));
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

        private async Task<Models.Device> GetDeviceFromDatabaseAsync(string id, string ownerId,
            bool includeNavigations = false)
        {
            if (!Guid.TryParse(id, out var deviceGuid) || !Guid.TryParse(ownerId, out var ownerGuid)) return null;
            if (includeNavigations)
            {
                return await _context.Devices
                    .Include(d => d.Location)
                    .Where(d => d.OwnerId == ownerGuid)
                    .FirstOrDefaultAsync(d => d.Id == deviceGuid);
            }

            return await _context.Devices
                .Where(d => d.OwnerId == ownerGuid)
                .FirstOrDefaultAsync(d => d.Id == deviceGuid);
        }
    }
}