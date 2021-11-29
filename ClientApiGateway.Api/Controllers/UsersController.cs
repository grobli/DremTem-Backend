using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.User;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;
using static ClientApiGateway.Api.Handlers.RpcExceptionHandler;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserGrpcService.UserGrpcServiceClient _userService;
        private readonly IMapper _mapper;

        public UsersController(
            ILogger<UsersController> logger,
            UserGrpcService.UserGrpcServiceClient userService,
            IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }


        // GET: api/v1/Users?pageNumber=1&pageSize=3
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers(
            [FromQuery] UserPagedParameters parameters, CancellationToken token)
        {
            var request = new GetAllUsersRequest
                { PageNumber = parameters.Page.Number, PageSize = parameters.Page.Size };
            try
            {
                var result = await _userService.GetAllUsersAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                return Ok(result.Users);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Users/fa87b04b-002f-4490-9c4d-659c474924cd
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            try
            {
                return Ok(await _userService.GetUserByIdAsync(new GetUserByIdRequest { Id = id.ToString() }));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Users/some%40email.com
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            try
            {
                return Ok(await _userService.GetUserByEmailAsync(new GetUserByEmailRequest { Email = email }));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Users/me
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(await _userService.GetUserByIdAsync(new GetUserByIdRequest { Id = userId }));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Users/fa87b04b-002f-4490-9c4d-659c474924cd
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserDto>> UpdateUserDetails(UpdateUserDetailsResource resource, Guid id)
        {
            var request = _mapper.Map<UpdateUserDetailsResource, UpdateUserDetailsRequest>(resource);
            request.Id = id.ToString();
            try
            {
                return Ok(await _userService.UpdateUserDetailsAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        //  PUT: api/v1/Users/me
        [HttpPut("me")]
        public async Task<ActionResult<UserDto>> UpdateCurrentUserDetails(UpdateUserDetailsResource resource)
        {
            var request = _mapper.Map<UpdateUserDetailsResource, UpdateUserDetailsRequest>(resource);
            request.Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                return Ok(await _userService.UpdateUserDetailsAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Users/fa87b04b-002f-4490-9c4d-659c474924cd
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<DeleteUserResponse>> DeleteUser(Guid id)
        {
            var request = new DeleteUserRequest { Id = id.ToString() };
            try
            {
                var result = await _userService.DeleteUserAsync(request);
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}