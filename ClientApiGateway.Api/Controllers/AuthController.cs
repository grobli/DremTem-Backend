using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Services.GrpcClientProvider;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;
using static ClientApiGateway.Api.Handlers.RpcExceptionHandler;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IGrpcClientProvider<UserAuthGrpcService.UserAuthGrpcServiceClient> _clientProvider;

        public AuthController(ILogger<AuthController> logger,
            IGrpcClientProvider<UserAuthGrpcService.UserAuthGrpcServiceClient> clientProvider)
        {
            _logger = logger;
            _clientProvider = clientProvider;
        }

        // POST: api/v1/Auth/signup
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<Empty>> SignUp(UserSignUpRequest request, CancellationToken token)
        {
            try
            {
                var result = await _clientProvider.SendRequestAsync(async client =>
                    await client.SignUpAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> SignIn(UserLoginRequest request, CancellationToken token)
        {
            try
            {
                var result = await _clientProvider.SendRequestAsync(async client =>
                    await client.SignInAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Auth/roles
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost("roles")]
        public async Task<ActionResult<Empty>> CreateRole(CreateRoleRequest request, CancellationToken token)
        {
            try
            {
                var result = await _clientProvider.SendRequestAsync(async client =>
                    await client.CreateRoleAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Auth/roles/assign
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost("roles/assign")]
        public async Task<ActionResult<Empty>> AddUserToRole(AddUserToRoleRequest request, CancellationToken token)
        {
            try
            {
                var result = await _clientProvider.SendRequestAsync(async client =>
                    await client.AddUserToRoleAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}