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
    public class LocationGrpcService : Core.Proto.LocationGrpcService.LocationGrpcServiceBase
    {
        private readonly ILogger<LocationGrpcService> _logger;
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;

        // validators
        private readonly IValidator<GenericGetManyRequest> _getAllValidator;
        private readonly IValidator<GenericGetRequest> _getValidator;
        private readonly IValidator<CreateLocationRequest> _createValidator;
        private readonly IValidator<UpdateLocationRequest> _updateValidator;
        private readonly IValidator<GenericDeleteRequest> _deleteValidator;

        public LocationGrpcService(
            ILogger<LocationGrpcService> logger,
            ILocationService locationService,
            IMapper mapper,
            IValidator<GenericGetManyRequest> getAllValidator,
            IValidator<GenericGetRequest> getValidator,
            IValidator<CreateLocationRequest> createValidator,
            IValidator<GenericDeleteRequest> deleteLocationValidator,
            IValidator<UpdateLocationRequest> updateValidator)
        {
            _logger = logger;
            _locationService = locationService;
            _mapper = mapper;

            _getAllValidator = getAllValidator;
            _getValidator = getValidator;
            _createValidator = createValidator;
            _deleteValidator = deleteLocationValidator;
            _updateValidator = updateValidator;
        }

        public override async Task<GetAllLocationsResponse> GetAllLocations(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getAllValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = request.Parameters?.UserId() ?? Guid.Empty;
            var locations = _locationService.GetAllLocations(userId);
            if (request.Parameters != null &&
                request.Parameters.IncludeFieldsSet(Entity.Device).Contains(Entity.Device))
            {
                locations = locations.Include(l => l.Devices);
            }

            var pagedList = await PagedList<Location>.ToPagedListAsync(locations, request.PageNumber, request.PageSize,
                context.CancellationToken);

            var response = new GetAllLocationsResponse
            {
                Locations = { pagedList.Select(l => _mapper.Map<Location, LocationResourceExtended>(l)) },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return await Task.FromResult(response);
        }

        public override async Task<LocationResourceExtended> GetLocation(GenericGetRequest request,
            ServerCallContext context)
        {
            var validationResult = await _getValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = request.Parameters?.UserId() ?? Guid.Empty;
            var locationQuery = _locationService.GetLocation(request.Id, userId);
            if (request.Parameters != null &&
                request.Parameters.IncludeFieldsSet(Entity.Device).Contains(Entity.Device))
            {
                locationQuery = locationQuery.Include(l => l.Devices);
            }

            var location = await locationQuery.SingleOrDefaultAsync(context.CancellationToken);
            if (location is null)
            {
                throw new RpcException(
                    new Status(StatusCode.NotFound, "Not found"));
            }

            return await Task.FromResult(_mapper.Map<Location, LocationResourceExtended>(location));
        }

        public override async Task<LocationResource> CreateLocation(CreateLocationRequest request,
            ServerCallContext context)
        {
            var validationResult = await _createValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newLocation = _mapper.Map<CreateLocationRequest, Location>(request);

            var createdLocation = await _locationService.CreateLocationAsync(newLocation);

            return await Task.FromResult(_mapper.Map<Location, LocationResource>(createdLocation));
        }

        public override async Task<LocationResource> UpdateLocation(UpdateLocationRequest request,
            ServerCallContext context)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var location = await _locationService.GetLocation(request.Id, request.UserId())
                .SingleOrDefaultAsync(context.CancellationToken);
            if (location is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _locationService
                .UpdateLocationAsync(location, _mapper.Map<UpdateLocationRequest, Location>(request));
            return await Task.FromResult(_mapper.Map<Location, LocationResource>(location));
        }

        public override async Task<Empty> DeleteLocation(GenericDeleteRequest request, ServerCallContext context)
        {
            var validationResult = await _deleteValidator.ValidateAsync(request, context.CancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var location = await _locationService.GetLocation(request.Id, request.UserId())
                .SingleOrDefaultAsync(context.CancellationToken);
            if (location is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _locationService.DeleteLocationAsync(location);
            return await Task.FromResult(new Empty());
        }
    }
}