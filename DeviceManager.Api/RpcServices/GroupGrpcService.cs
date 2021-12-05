using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Api.Queries;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Group;

namespace DeviceManager.Api.RpcServices
{
    public class GroupGrpcService : GroupGrpc.GroupGrpcBase
    {
        private readonly ILogger<GroupGrpcService> _logger;
        private readonly IMediator _mediator;

        public GroupGrpcService(ILogger<GroupGrpcService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<GetAllGroupsResponse> GetAllGroups(GenericGetManyRequest request,
            ServerCallContext context)
        {
            var query = new GetAllGroupsQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<GroupDto> GetGroup(GenericGetRequest request, ServerCallContext context)
        {
            var query = new GetGroupQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<GroupDto> CreateGroup(CreateGroupRequest request, ServerCallContext context)
        {
            var command = new CreateGroupCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<GroupDto> UpdateGroup(UpdateGroupRequest request, ServerCallContext context)
        {
            var command = new UpdateGroupCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<Empty> DeleteGroup(GenericDeleteRequest request, ServerCallContext context)
        {
            var command = new DeleteGroupCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<Empty> AddDeviceToGroup(AddDeviceToGroupRequest request, ServerCallContext context)
        {
            var command = new AddDeviceToGroupCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<Empty> RemoveDeviceFromGroup(RemoveDeviceFromGroupRequest request,
            ServerCallContext context)
        {
            var command = new RemoveDeviceFromGroupCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }
    }
}