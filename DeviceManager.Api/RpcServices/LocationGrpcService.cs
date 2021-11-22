using System;
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
    public class LocationGrpcService : Core.Proto.LocationGrpcService.LocationGrpcServiceBase
    {
        private readonly ILogger<LocationGrpcService> _logger;
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;

        // validators
        private readonly IValidator<GetAllLocationsRequest> _getAllLocationsValidator;
        private readonly IValidator<GetLocationRequest> _getLocationValidator;
        private readonly IValidator<SaveLocationRequest> _saveLocationValidator;
        private readonly IValidator<DeleteLocationRequest> _deleteLocationValidator;

        public LocationGrpcService(
            ILogger<LocationGrpcService> logger,
            ILocationService locationService,
            IMapper mapper,
            IValidator<GetAllLocationsRequest> getAllLocationsValidator,
            IValidator<GetLocationRequest> getLocationValidator,
            IValidator<SaveLocationRequest> saveLocationValidator,
            IValidator<DeleteLocationRequest> deleteLocationValidator)
        {
            _logger = logger;
            _locationService = locationService;
            _mapper = mapper;

            _getAllLocationsValidator = getAllLocationsValidator;
            _getLocationValidator = getLocationValidator;
            _saveLocationValidator = saveLocationValidator;
            _deleteLocationValidator = deleteLocationValidator;
        }

        public override async Task GetAllLocations(GetAllLocationsRequest request,
            IServerStreamWriter<LocationResource> responseStream, ServerCallContext context)
        {
            var validationResult = await _getAllLocationsValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            Guid? userId = string.IsNullOrWhiteSpace(request.UserId)
                ? null
                : Guid.Parse(request.UserId);

            var locations = request.IncludeDevices
                ? await _locationService.GetAllLocationsWithDevices(userId)
                : await _locationService.GetAllLocations(userId);

            foreach (var location in locations)
            {
                await responseStream.WriteAsync(_mapper.Map<Location, LocationResource>(location));
            }
        }

        public override async Task<LocationResource> GetLocation(GetLocationRequest request, ServerCallContext context)
        {
            var validationResult = await _getLocationValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(request.UserId);
            var location = request.IncludeDevices
                ? await _locationService.GetLocationWithDevices(userId, request.Name)
                : await _locationService.GetLocation(userId, request.Name);

            if (location is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Location with Name = {request.Name} not found"));
            }

            return await Task.FromResult(_mapper.Map<Location, LocationResource>(location));
        }

        public override async Task<LocationResource> CreateLocation(SaveLocationRequest request,
            ServerCallContext context)
        {
            var validationResult = await _saveLocationValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newLocation = _mapper.Map<SaveLocationRequest, Location>(request);

            var createdLocation = await _locationService.CreateLocation(newLocation);

            return await Task.FromResult(_mapper.Map<Location, LocationResource>(createdLocation));
        }

        public override async Task<LocationResource> UpdateLocation(SaveLocationRequest request,
            ServerCallContext context)
        {
            var validationResult = await _saveLocationValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(request.UserId);
            var location = await _locationService.GetLocation(userId, request.Name);
            if (location is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Location with Name = {request.Name} not found"));
            }

            await _locationService
                .UpdateLocation(location, _mapper.Map<SaveLocationRequest, Location>(request));

            return await Task.FromResult(_mapper.Map<Location, LocationResource>(location));
        }

        public override async Task<Empty> DeleteLocation(DeleteLocationRequest request, ServerCallContext context)
        {
            var validationResult = await _deleteLocationValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = Guid.Parse(request.UserId);
            var location = await _locationService.GetLocation(userId, request.Name);
            if (location is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Location with Name = {request.Name} not found"));
            }

            await _locationService.DeleteLocation(location);

            return await Task.FromResult(new Empty());
        }
    }
}