using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Proto.Common;

namespace DeviceManager.Api.Handlers.LocationHandlers
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, Empty>
    {
        private readonly ILocationService _locationService;
        private readonly IValidator<GenericDeleteRequest> _validator;

        public DeleteLocationHandler(ILocationService locationService, IValidator<GenericDeleteRequest> validator)
        {
            _locationService = locationService;
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