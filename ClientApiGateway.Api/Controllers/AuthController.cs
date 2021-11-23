using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserAuthGrpcService.UserAuthGrpcServiceClient _authService;

        public AuthController(
            ILogger<AuthController> logger,
            UserAuthGrpcService.UserAuthGrpcServiceClient authService)
        {
            _logger = logger;
            _authService = authService;
        }

        // POST: api/v1/Auth/signup
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<Empty>> SignUp(UserSignUpRequest request)
        {
            try
            {
                return Ok(await _authService.SignUpAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // POST: api/v1/Auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> SignIn(UserLoginRequest request)
        {
            try
            {
                return Ok(await _authService.SignInAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // POST: api/v1/Auth/roles
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost("roles")]
        public async Task<ActionResult<Empty>> CreateRole(CreateRoleRequest request)
        {
            try
            {
                return Ok(await _authService.CreateRoleAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // POST: api/v1/Auth/roles/assign
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost("roles/assign")]
        public async Task<ActionResult<Empty>> AddUserToRole(AddUserToRoleRequest request)
        {
            try
            {
                return Ok(await _authService.AddUserToRoleAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }
    }
}