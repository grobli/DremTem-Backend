using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Proto;

namespace WebApiGateway.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableQuery]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserInfoGrpc.UserInfoGrpcClient _userClient;

        public UserController(ILogger<UserController> logger, UserInfoGrpc.UserInfoGrpcClient userClient)
        {
            _logger = logger;
            _userClient = userClient;
        }

        // GET: api/v1/User/info
        [HttpGet("current/info")]
        public async Task<ActionResult<UserInfoResource>> GetUserByEmail()
        {
            var email = HttpContext.User.Identity?.Name;
            if (email is null)
            {
                return BadRequest();
            }

            try
            {
                var userInfo = await _userClient.GetUserByEmailAsync(new GetUserByEmailRequest { Email = email });
                return Ok(userInfo);
            }
            catch (RpcException e)
            {
                return Problem(e.Status.ToString());
            }
        }

        // POST: api/v1/User/de2fd5b2-fb6e-4698-943a-5e840489fcec/edit/firstName
        [HttpPost("{id:guid}/edit/firstName")]
        public async Task<ActionResult<UserInfoResource>> UpdateUserFirstName(Guid id, [FromBody] string firstName)
        {
            try
            {
                var result = await _userClient.UpdateUserFirstNameAsync(new UpdateUserFirstNameRequest
                {
                    Id = id.ToString(),
                    FirstName = firstName
                });
                return Ok(result);
            }
            catch (RpcException e)
            {
                return Problem(e.Status.ToString());
            }
        }

        // POST: api/v1/User/de2fd5b2-fb6e-4698-943a-5e840489fcec/edit/lastName
        [HttpPost("{id:guid}/edit/lastName")]
        public async Task<ActionResult<UserInfoResource>> UpdateUserLastName(Guid id, [FromBody] string lastName)
        {
            try
            {
                var result = await _userClient.UpdateUserLastNameAsync(new UpdateUserLastNameRequest
                {
                    Id = id.ToString(),
                    LastName = lastName
                });
                return Ok(result);
            }
            catch (RpcException e)
            {
                return Problem(e.Status.ToString());
            }
        }
    }
}