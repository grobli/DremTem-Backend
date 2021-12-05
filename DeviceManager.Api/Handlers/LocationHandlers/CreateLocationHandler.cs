using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Shared.Proto;
using Shared.Proto.Location;

namespace DeviceManager.Api.Handlers.LocationHandlers
{
    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, LocationDto>
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateLocationRequest> _validator;

        public CreateLocationHandler(ILocationService locationService, IMapper mapper,
            IValidator<CreateLocationRequest> validator)
        {
            _locationService = locationService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<LocationDto> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newLocation = _mapper.Map<CreateLocationRequest, Location>(request.Body);

            try
            {
                var createdLocation = await _locationService.CreateLocationAsync(newLocation, cancellationToken);
                return _mapper.Map<Location, LocationDto>(createdLocation);
            }
            catch (ValidationException e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}