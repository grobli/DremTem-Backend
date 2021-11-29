using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using UserIdentity.Api.Commands;
using UserIdentity.Api.Queries;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Services.Rpc
{
    public class UserService : UserGrpcService.UserGrpcServiceBase
    {
        private readonly ILogger<UserService> _logger;
        private readonly IMediator _mediator;

        public UserService(ILogger<UserService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<GetAllUsersResponse> GetAllUsers(GetAllUsersRequest request,
            ServerCallContext context)
        {
            var query = new GetAllUsersQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<UserDto> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var query = new GetUserByIdQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<UserDto> GetUserByEmail(GetUserByEmailRequest request,
            ServerCallContext context)
        {
            var query = new GetUserByEmailQuery(request);
            var result = await _mediator.Send(query, context.CancellationToken);
            return result;
        }

        public override async Task<UserDto> UpdateUserDetails(UpdateUserDetailsRequest request,
            ServerCallContext context)
        {
            var command = new UpdateUserDetailsCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            var command = new DeleteUserCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }
    }
}