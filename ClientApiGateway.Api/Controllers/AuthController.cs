using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ClientApiGateway.Api.Resources;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Proto;
using Shared.Services.GrpcClientServices;
using UserIdentity.Core.Models.Auth;
using static ClientApiGateway.Api.ExceptionHandlers.RpcExceptionHandler;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IGrpcService<UserAuthGrpc.UserAuthGrpcClient> _grpcService;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public AuthController(ILogger<AuthController> logger,
            IGrpcService<UserAuthGrpc.UserAuthGrpcClient> grpcService)
        {
            _logger = logger;
            _grpcService = grpcService;
        }

        // POST: api/v1/auth/signup
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<Empty>> SignUp(UserSignUpRequest request, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("User signup request: {Email}", request.Email);
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.SignUpAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> SignIn(UserLoginRequest request, CancellationToken token)
        {
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.SignInAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/auth/roles
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost("roles")]
        public async Task<ActionResult<Empty>> CreateRole(CreateRoleRequest request, CancellationToken token)
        {
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.CreateRoleAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/auth/roles/assign
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost("roles/assign")]
        public async Task<ActionResult<Empty>> AddUserToRole(AddUserToRoleRequest request, CancellationToken token)
        {
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.AddUserToRoleAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST api/v1/auth/change-password
        [HttpPost("change-password")]
        public async Task<ActionResult<Empty>> ChangePassword(ChangePasswordResource resource, CancellationToken token)
        {
            try
            {
                var request = new ChangePasswordRequest
                {
                    NewPassword = resource.NewPassword,
                    OldPassword = resource.OldPassword,
                    UserId = UserId
                };
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.ChangePasswordAsync(request));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}