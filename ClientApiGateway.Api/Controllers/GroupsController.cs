using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.Group;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Services.GrpcClientServices;
using UserIdentity.Core.Models.Auth;
using static ClientApiGateway.Api.Handlers.RpcExceptionHandler;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;
        private readonly IGrpcService<GroupGrpc.GroupGrpcClient> _grpcService;
        private readonly IMapper _mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public GroupsController(ILogger<GroupsController> logger, IGrpcService<GroupGrpc.GroupGrpcClient> grpcService,
            IMapper mapper)
        {
            _logger = logger;
            _grpcService = grpcService;
            _mapper = mapper;
        }

        // GET: api/v1/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetAllGroups(
            [FromQuery] GroupPagedParameters parameters, CancellationToken token)
        {
            return await GetAllGroups(parameters, true, token);
        }

        // GET: api/v1/Groups/all
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetAllGroupsOfAllUsers(
            [FromQuery] GroupPagedParameters parameters, CancellationToken token)
        {
            return await GetAllGroups(parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<GroupDto>>> GetAllGroups(
            [FromQuery] GroupPagedParameters parameters, bool limitToUser, CancellationToken token)
        {
            var request = new GenericGetManyRequest
            {
                Parameters = new GetRequestParameters
                {
                    UserId = limitToUser ? UserId : null
                },
                PageNumber = parameters.Page.Number,
                PageSize = parameters.Page.Size
            };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.GetAllGroupsAsync(request, cancellationToken: token));
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                return Ok(result.Groups);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Groups/42
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id, CancellationToken token)
        {
            var request = new GenericGetRequest
            {
                Id = id,
                Parameters = new GetRequestParameters
                {
                    UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
                }
            };
            try
            {
                var response = await _grpcService.SendRequestAsync(async client =>
                    await client.GetGroupAsync(request, cancellationToken: token));
                return Ok(response);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Groups
        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupResource resource, CancellationToken token)
        {
            var request = _mapper.Map<CreateGroupResource, CreateGroupRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdGroup = await _grpcService.SendRequestAsync(async client =>
                    await client.CreateGroupAsync(request, cancellationToken: token));
                return CreatedAtAction("GetGroup", new { id = createdGroup.Id }, createdGroup);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Groups/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<GroupDto>> UpdateGroup(UpdateGroupResource resource, int id,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateGroupResource, UpdateGroupRequest>(resource);
            request.Id = id;
            request.UserId = UserId;
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.UpdateGroupAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE api/v1/Groups/42
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Empty>> DeleteGroup(int id, CancellationToken token)
        {
            var request = new GenericDeleteRequest { Id = id, UserId = UserId };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.DeleteGroupAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST api/v1/Groups/42/add/2137
        [HttpPost("{groupId:int}/add/{deviceId:int}")]
        public async Task<ActionResult<Empty>> AddDeviceToGroup(int groupId, int deviceId,
            CancellationToken token)
        {
            var request = new AddDeviceToGroupRequest { DeviceId = deviceId, GroupId = groupId, UserId = UserId };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.AddDeviceToGroupAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE api/v1/Groups/42/remove/2137
        [HttpDelete("{groupId:int}/remove/{deviceId:int}")]
        public async Task<ActionResult<Empty>> RemoveDeviceFromGroup(int groupId, int deviceId,
            CancellationToken token)
        {
            var request = new RemoveDeviceFromGroupRequest { DeviceId = deviceId, GroupId = groupId, UserId = UserId };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.RemoveDeviceFromGroupAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}