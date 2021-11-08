using System;
using System.Threading.Tasks;
using AutoMapper;
using DeviceGrpcService.Data;
using DeviceGrpcService.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DeviceGrpcService.Services
{
    public class LocationService : Location.LocationBase
    {
        private readonly ILogger<LocationService> _logger;
        private readonly DeviceContext _context;
        private readonly IMapper _mapper;

        public LocationService(ILogger<LocationService> logger, DeviceContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetAllLocations(Empty request, IServerStreamWriter<LocationGrpcModel> responseStream,
            ServerCallContext context)
        {
            foreach (var location in _context.Locations)
            {
                await responseStream.WriteAsync(_mapper.Map<LocationGrpcModel>(location));
            }
        }

        public override async Task<LocationGrpcModel> GetLocationById(LocationByIdRequest request,
            ServerCallContext context)
        {
            var location = await _context.Locations.FindAsync(request.ID);
            if (location == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified location with ID={request.ID} does not exist"));
            }

            return await Task.FromResult(_mapper.Map<LocationGrpcModel>(location));
        }

        public override async Task<LocationGrpcModel> CreateLocation(LocationCreateRequest request,
            ServerCallContext context)
        {
            var name = request.Name;
            if (string.IsNullOrEmpty(name))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "Location name cannot be null or empty"));
            }

            var cords = GetLocationCordsFromRequest(request);
            var location = new Models.Location
                { Name = name, Latitude = cords?.Latitude, Longitude = cords?.Longitude };

            try
            {
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Unable to save changes");
                throw new RpcException(new Status(StatusCode.Internal,
                    "Unable to save changes." +
                    "Try again, and if the problem persists see your system administrator."));
            }

            return await Task.FromResult(_mapper.Map<LocationGrpcModel>(location));
        }

        private static LocationCords GetLocationCordsFromRequest(LocationCreateRequest request) =>
            request.CordsCase switch
            {
                LocationCreateRequest.CordsOneofCase.None => null,
                LocationCreateRequest.CordsOneofCase.CordsValue => request.CordsValue,
                _ => throw new ArgumentException("Invalid message - CordsValue")
            };


        private static (LocationCords cords, bool clearFlag)
            GetLocationCordsFromRequest(LocationUpdateRequest request) =>
            request.CordsCase switch
            {
                LocationUpdateRequest.CordsOneofCase.None => (null, false),
                LocationUpdateRequest.CordsOneofCase.CordsValue => (request.CordsValue, false),
                LocationUpdateRequest.CordsOneofCase.CordsNullified => (null, request.CordsNullified),
                _ => throw new ArgumentException("Invalid message - CordsValue")
            };

        private static string GetNameValueFromRequest(LocationUpdateRequest request) => request.NameCase switch
        {
            LocationUpdateRequest.NameOneofCase.None => null,
            LocationUpdateRequest.NameOneofCase.NameValue => request.NameValue,
            _ => throw new ArgumentException("Invalid message - NameValue")
        };

        public override async Task<LocationGrpcModel> UpdateLocationByID(LocationUpdateRequest request,
            ServerCallContext context)
        {
            var location = await _context.Locations.FindAsync(request.ID);
            if (location == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified location with ID={request.ID} does not exist"));
            }

            var name = GetNameValueFromRequest(request);
            if (name != null && name.Trim().Length == 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "Location name cannot be null or empty"));
            }

            var (cords, clearFlag) = GetLocationCordsFromRequest(request);

            location.Name = name ?? location.Name;
            location.Latitude = clearFlag ? null : cords?.Latitude ?? location.Latitude;
            location.Longitude = clearFlag ? null : cords?.Longitude ?? location.Longitude;

            try
            {
                _context.Locations.Update(location);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e, "Unable to save changes");
                throw new RpcException(new Status(StatusCode.Internal,
                    "Unable to save changes." +
                    "Try again, and if the problem persists see your system administrator."));
            }

            return await Task.FromResult(_mapper.Map<LocationGrpcModel>(location));
        }

        public override async Task<Empty> DeleteLocationByID(LocationByIdRequest request, ServerCallContext context)
        {
            var location = await _context.Locations.FindAsync(request.ID);
            if (location == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Specified location with ID={request.ID} does not exist"));
            }

            try
            {
                _context.Locations.Remove(location);
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