using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientApiGateway.Api.Resources;
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
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserGrpcService.UserGrpcServiceClient _userService;

        public UsersController(
            ILogger<UsersController> logger,
            UserGrpcService.UserGrpcServiceClient userService)
        {
            _logger = logger;
            _userService = userService;
        }

        // GET: api/v1/Users
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResource>>> GetAllUsers()
        {
            try
            {
                var users = new List<UserResource>();

                var call = _userService.GetAllUsers(new GetAllUsersRequest());
                await foreach (var user in call.ResponseStream.ReadAllAsync())
                {
                    users.Add(user);
                }

                return Ok(users);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Users/fa87b04b-002f-4490-9c4d-659c474924cd
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserResource>> GetUserById(Guid id)
        {
            try
            {
                return Ok(await _userService.GetUserByIdAsync(new GetUserByIdRequest { Id = id.ToString() }));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Users/some%40email.com
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("{email}")]
        public async Task<ActionResult<UserResource>> GetUserByEmail(string email)
        {
            try
            {
                return Ok(await _userService.GetUserByEmailAsync(new GetUserByEmailRequest() { Email = email }));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Users/me
        [HttpGet("me")]
        public async Task<ActionResult<UserResource>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await _userService.GetUserByIdAsync(new GetUserByIdRequest() { Id = userId }));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // PUT: api/v1/Users/fa87b04b-002f-4490-9c4d-659c474924cd
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserResource>> UpdateUserDetails(UpdateUserDetailsResource resource, Guid id)
        {
            try
            {
                var (firstName, lastName) = resource;
                var request = new UpdateUserDetailsRequest
                {
                    Id = id.ToString(),
                    FirstName = firstName,
                    LastName = lastName
                };

                return Ok(await _userService.UpdateUserDetailsAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        //  PUT: api/v1/Users/me
        [HttpPut("me")]
        public async Task<ActionResult<UserResource>> UpdateCurrentUserDetails(UpdateUserDetailsResource resource)
        {
            var (firstName, lastName) = resource;
            var request = new UpdateUserDetailsRequest
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                FirstName = firstName,
                LastName = lastName
            };

            try
            {
                return Ok(await _userService.UpdateUserDetailsAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }
    }
}