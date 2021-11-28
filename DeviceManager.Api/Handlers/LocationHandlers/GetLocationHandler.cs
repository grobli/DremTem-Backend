using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeviceManager.Core.Extensions;
using DeviceManager.Core.Models;

namespace DeviceManager.Api.Handlers.LocationHandlers
{
    public class GetLocationHandler : IRequestHandler<GetLocationQuery, LocationExtendedDto>
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericGetRequest> _validator;

        public GetLocationHandler(ILocationService locationService, IValidator<GenericGetRequest> validator,
            IMapper mapper)
        {
            _locationService = locationService;
            _validator = validator;
            _mapper = mapper;
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

            return _mapper.Map<Location, LocationExtendedDto>(location);
        }
    }
}