using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DeviceManager.Api.Handlers.LocationHandlers
{
    public class GetAllLocationsHandler : IRequestHandler<GetAllLocationsQuery, GetAllLocationsResponse>
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetManyRequest> _validator;

        public GetAllLocationsHandler(ILocationService locationService, IMapper mapper,
            IValidator<GenericGetManyRequest> validator)
        {
            _locationService = locationService;
            _mapper = mapper;
            _validator = validator;
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

            var response = new GetAllLocationsResponse
            {
                Locations = { pagedList.Select(l => _mapper.Map<Location, LocationExtendedDto>(l)) },
                MetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;
        }
    }
}