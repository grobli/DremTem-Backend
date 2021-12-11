using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeviceManager.Core.Models;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Handlers.LocationHandlers
{
    public class GetLocationHandler : IRequestHandler<GetLocationQuery, LocationExtendedDto>
    {
        private readonly ILocationService _locationService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetRequest> _validator;

        public GetLocationHandler(ILocationService locationService, IValidator<GenericGetRequest> validator,
            IMapper mapper, IDeviceService deviceService)
        {
            _locationService = locationService;
            _validator = validator;
            _mapper = mapper;
            _deviceService = deviceService;
        }

        public async Task<LocationExtendedDto> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            var locationQuery = _locationService.GetLocationQuery(query.Id, userId);
            if (query.Parameters != null &&
                query.Parameters.IncludeFieldsSet(Entity.Device).Contains(Entity.Device))
            {
                locationQuery = locationQuery.Include(l => l.Devices);
            }

            var location = await locationQuery.SingleOrDefaultAsync(cancellationToken);
            if (location is null)
            {
                throw new RpcException(
                    new Status(StatusCode.NotFound, "Not found"));
            }

            var locationMap = _mapper.Map<Location, LocationExtendedDto>(location);

            // add device ids
            var devices = query.Parameters.IncludeFieldsSet(Entity.Device).Count > 0
                ? locationMap.Devices.Select(d => d.Id)
                : await _deviceService.GetAllDevicesQuery(userId)
                    .Where(d => d.LocationId == location.Id)
                    .Select(d => d.Id)
                    .ToArrayAsync(cancellationToken);
            locationMap.DeviceIds.AddRange(devices);

            return locationMap;
        }
    }
}