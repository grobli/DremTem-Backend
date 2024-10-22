﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
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
    public class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, LocationDto>
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateLocationRequest> _validator;

        public UpdateLocationHandler(ILocationService locationService, IMapper mapper,
            IValidator<UpdateLocationRequest> validator)
        {
            _locationService = locationService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<LocationDto> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var location = await _locationService.GetLocationQuery(request.Body.Id, request.Body.UserId())
                .SingleOrDefaultAsync(cancellationToken);
            if (location is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            try
            {
                await _locationService
                    .UpdateLocationAsync(location, _mapper.Map<UpdateLocationRequest, Location>(request.Body),
                        cancellationToken);
                return _mapper.Map<Location, LocationDto>(location);
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}