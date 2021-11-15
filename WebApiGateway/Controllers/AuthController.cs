using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Proto;

namespace WebApiGateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserAuthGrpc.UserAuthGrpcClient _authClient;

        public AuthController(ILogger<AuthController> logger, UserAuthGrpc.UserAuthGrpcClient authClient)
        {
            _logger = logger;
            _authClient = authClient;
        }

        // POST: api/v1/AuthController/SignUp
        [HttpPost("SignUp")]
        public async Task<ActionResult<Empty>> SignUp(UserSignUpRequest request)
        {
            try
            {
                await _authClient.SignUpAsync(request);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }

            return Ok();
        }

        // POST: api/v1/AuthController/SignIn
        [HttpPost("SignIn")]
        public async Task<ActionResult<UserLoginResponse>> SignIn(UserLoginRequest request)
        {
            try
            {
                var result = await _authClient.SignInAsync(request);
                return Ok(result);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }
    }
}