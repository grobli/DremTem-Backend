using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Extensions;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Location;

namespace DeviceManager.Api.Handlers.LocationHandlers
{
    public class GetAllLocationsHandler : IRequestHandler<GetAllLocationsQuery, GetAllLocationsResponse>
    {
        private readonly ILocationService _locationService;
        private readonly IDeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetManyRequest> _validator;

        public GetAllLocationsHandler(ILocationService locationService, IMapper mapper,
            IValidator<GenericGetManyRequest> validator, IDeviceService deviceService)
        {
            _locationService = locationService;
            _mapper = mapper;
            _validator = validator;
            _deviceService = deviceService;
        }

        public async Task<GetAllLocationsResponse> Handle(GetAllLocationsQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.QueryParameters;
            var validationResult = await _validator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var userId = query.Parameters?.UserId() ?? Guid.Empty;
            var locations = _locationService.GetAllLocationsQuery(userId);
            if (query.Parameters != null &&
                query.Parameters.IncludeFieldsSet(Entity.Device).Contains(Entity.Device))
            {
                locations = locations.Include(l => l.Devices);
            }

            var pagedList = await PagedList<Location>.ToPagedListAsync(locations, query.PageNumber, query.PageSize,
                cancellationToken);

            var pagedListMapped = pagedList
                .Select(l => _mapper.Map<Location, LocationExtendedDto>(l))
                .ToList();

            await AddDeviceIdsToMap();

            var response = new GetAllLocationsResponse
            {
                Locations = { pagedListMapped },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;

            async Task AddDeviceIdsToMap()
            {
                var locationIds = pagedListMapped.Select(l => l.Id);
                var devices = await _deviceService.GetAllDevicesQuery(userId)
                    .Where(d => locationIds.Contains(d.LocationId ?? 0))
                    .Select(d => new { LocationId = d.LocationId.Value, d.Id })
                    .ToListAsync(cancellationToken);

                var devicesDict = devices
                    .GroupBy(d => d.LocationId)
                    .Select(pair => new { LocationId = pair.Key, DeviceIds = pair.Select(p => p.Id) })
                    .ToDictionary(x => x.LocationId, x => x.DeviceIds);

                foreach (var location in pagedListMapped)
                {
                    if (devicesDict.TryGetValue(location.Id, out var deviceIds))
                        location.DeviceIds.AddRange(deviceIds);
                }
            }
        }
    }
}