using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using UserIdentity.Api.Commands;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Services.Rpc
{
    public class AuthService : UserAuthGrpcService.UserAuthGrpcServiceBase
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IMediator _mediator;

        public AuthService(IMediator mediator, ILogger<AuthService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<Empty> SignUp(UserSignUpRequest request, ServerCallContext context)
        {
            var command = new SignUpCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<UserLoginResponse> SignIn(UserLoginRequest request, ServerCallContext context)
        {
            var command = new LoginCommand(request);
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<Empty> CreateRole(CreateRoleRequest request, ServerCallContext context)
        {
            var command = new CreateRoleCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<Empty> AddUserToRole(AddUserToRoleRequest request, ServerCallContext context)
        {
            var command = new AddUserToRoleCommand(request);
            return await _mediator.Send(command, context.CancellationToken);
        }
    }
}