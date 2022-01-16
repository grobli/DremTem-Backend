using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Proto;
using Shared.Services.GrpcClientServices;
using UserIdentity.Core.Models.Auth;
using static ClientApiGateway.Api.ExceptionHandlers.RpcExceptionHandler;

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
        private readonly IAppCache _cache;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private bool DenyCache => string.Equals(Request.Headers["Cache-Control"], "no-cache",
            StringComparison.InvariantCultureIgnoreCase);


        public GroupsController(ILogger<GroupsController> logger, IGrpcService<GroupGrpc.GroupGrpcClient> grpcService,
            IMapper mapper, IAppCache cache)
        {
            _logger = logger;
            _grpcService = grpcService;
            _mapper = mapper;
            _cache = cache;
        }

        // GET: api/v1/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupResource>>> GetAllGroups(
            [FromQuery] PaginationParameters pagination, CancellationToken token)
        {
            return await GetAllGroups(pagination, true, token);
        }

        // GET: api/v1/Groups/all
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GroupResource>>> GetAllGroupsOfAllUsers(
            [FromQuery] PaginationParameters pagination, CancellationToken token)
        {
            return await GetAllGroups(pagination, false, token);
        }

        private async Task<ActionResult<IEnumerable<GroupResource>>> GetAllGroups(
            [FromQuery] PaginationParameters pagination, bool limitToUser, CancellationToken token)
        {
            var request = new GenericGetManyRequest
            {
                Parameters = new GetRequestParameters
                {
                    UserId = limitToUser ? UserId : null
                },
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
            var cacheKey = $"{nameof(GetAllGroups)}{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                GetAllGroupsResponse result;
                if (DenyCache)
                {
                    result = await GetAll();
                    _cache.Add(cacheKey, result, cacheTimespan);
                }
                else
                {
                    result = await _cache.GetOrAddAsync(cacheKey, GetAll, cacheTimespan);
                }

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                var resources = result.Groups.Select(g => _mapper.Map<GroupDto, GroupResource>(g));
                return Ok(resources);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<GetAllGroupsResponse> GetAll()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetAllGroupsAsync(request, cancellationToken: token));
            }
        }

        // GET: api/v1/Groups/42
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupResource>> GetGroup(int id, CancellationToken token)
        {
            var request = new GenericGetRequest
            {
                Id = id,
                Parameters = new GetRequestParameters
                {
                    UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
                }
            };
            var cacheKey = $"{nameof(GetGroup)}{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                GroupDto result;
                if (DenyCache)
                {
                    result = await Get();
                    _cache.Add(cacheKey, result, cacheTimespan);
                }
                else
                {
                    result = await _cache.GetOrAddAsync(cacheKey, Get, cacheTimespan);
                }

                return Ok(_mapper.Map<GroupDto, GroupResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<GroupDto> Get()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetGroupAsync(request, cancellationToken: token));
            }
        }

        // POST: api/v1/Groups
        [HttpPost]
        public async Task<ActionResult<GroupResource>> CreateGroup(CreateGroupResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<CreateGroupResource, CreateGroupRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdGroup = await _grpcService.SendRequestAsync(async client =>
                    await client.CreateGroupAsync(request, cancellationToken: token));
                return CreatedAtAction("GetGroup", new { id = createdGroup.Id },
                    _mapper.Map<GroupDto, GroupResource>(createdGroup));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Groups/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<GroupResource>> UpdateGroup(UpdateGroupResource resource, int id,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateGroupResource, UpdateGroupRequest>(resource);
            request.Id = id;
            request.UserId = UserId;
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.UpdateGroupAsync(request, cancellationToken: token));
                return Ok(_mapper.Map<GroupDto, GroupResource>(result));
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