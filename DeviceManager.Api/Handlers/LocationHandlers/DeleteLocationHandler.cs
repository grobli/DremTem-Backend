using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeviceManager.Core.Extensions;

namespace DeviceManager.Api.Handlers.LocationHandlers
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, Empty>
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericDeleteRequest> _validator;

        public DeleteLocationHandler(ILocationService locationService, IMapper mapper,
            IValidator<GenericDeleteRequest> validator)
        {
            _locationService = locationService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<Empty> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
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

            await _locationService.DeleteLocationAsync(location, cancellationToken);
            return await Task.FromResult(new Empty());
        }
    }
}