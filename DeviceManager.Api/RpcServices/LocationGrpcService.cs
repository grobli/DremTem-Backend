using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Api.Queries;
using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Api.RpcServices
{
    public class LocationGrpcService : Core.Proto.LocationGrpcService.LocationGrpcServiceBase
    {
        private ILogger<LocationGrpcService> _logger;
        private readonly IMediator _mediator;

        public LocationGrpcService(IMediator mediator, ILogger<LocationGrpcService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<GetAllLocationsResponse> GetAllLocations(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var query = new GetAllLocationsQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<LocationDtoExtended> GetLocation(GenericGetRequest request,
            ServerCallContext context)
        {
            var query = new GetLocationQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<LocationDto> CreateLocation(CreateLocationRequest request,
            ServerCallContext context)
        {
            var command = new CreateLocationCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<LocationDto> UpdateLocation(UpdateLocationRequest request,
            ServerCallContext context)
        {
            var command = new UpdateLocationCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<Empty> DeleteLocation(GenericDeleteRequest request, ServerCallContext context)
        {
            var command = new DeleteLocationCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }
    }
}