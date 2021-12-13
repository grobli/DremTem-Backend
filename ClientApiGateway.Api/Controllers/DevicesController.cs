using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources;
using DeviceManager.Core.Models;
using Grpc.Core;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Proto;
using Shared.Services.GrpcClientServices;
using UserIdentity.Core.Models.Auth;
using static ClientApiGateway.Api.Handlers.RpcExceptionHandler;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;
        private readonly IAppCache _cache;
        private readonly IMapper _mapper;
        private readonly IGrpcService<DeviceGrpc.DeviceGrpcClient> _grpcService;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private bool DenyCache => string.Equals(Request.Headers["Cache-Control"], "no-cache",
            StringComparison.InvariantCultureIgnoreCase);

        public DevicesController(
            ILogger<DevicesController> logger, IMapper mapper,
            IGrpcService<DeviceGrpc.DeviceGrpcClient> grpcService, IAppCache cache)
        {
            _logger = logger;
            _mapper = mapper;
            _grpcService = grpcService;
            _cache = cache;
        }

        // GET: api/v1/devices?pageNumber=1&pageSize=3&includeLocation=true&includeSensors=false
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResource>>> GetAllDevices(
            [FromQuery] PaginationParameters pagination, [FromQuery] DeviceParameters parameters,
            CancellationToken token)
        {
            return await GetAllDevices(pagination, parameters, true, token);
        }

        // GET: api/v1/devices/all?includeLocation=true&includeSensors=false
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DeviceResource>>> GetAllDevicesOfAllUsers(
            [FromQuery] PaginationParameters pagination, [FromQuery] DeviceParameters parameters,
            CancellationToken token)
        {
            return await GetAllDevices(pagination, parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<DeviceResource>>> GetAllDevices(
            PaginationParameters pagination, DeviceParameters parameters, bool limitToUser,
            CancellationToken token)
        {
            var request = new GenericGetManyRequest
            {
                Parameters = new GetRequestParameters
                {
                    UserId = limitToUser ? UserId : null,
                    IncludeFields = { parameters.FieldsToInclude() }
                },
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
            var cacheKey = $"{nameof(GetAllDevices)}-{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                GetAllDevicesResponse result;
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

                var resources = result.Devices
                    .Select(d => _mapper.Map<DeviceExtendedDto, DeviceResource>(d));
                return Ok(resources);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<GetAllDevicesResponse> GetAll()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetAllDevicesAsync(request, cancellationToken: token));
            }
        }

        // GET: api/v1/Devices/id/42?includeLocation=true
        [HttpGet("id/{id:int}")]
        public async Task<ActionResult<DeviceResource>> GetDevice(int id,
            [FromQuery] DeviceParameters parameters, CancellationToken token)
        {
            var request = new GenericGetRequest
            {
                Id = id,
                Parameters = new GetRequestParameters
                {
                    UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId,
                    IncludeFields = { parameters.FieldsToInclude() }
                }
            };
            var cacheKey = $"{nameof(GetDevice)}-{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                DeviceExtendedDto result;
                if (DenyCache)
                {
                    result = await Get();
                    _cache.Add(cacheKey, result, cacheTimespan);
                }
                else
                {
                    result = await _cache.GetOrAddAsync(cacheKey, Get, cacheTimespan);
                }

                var resource = _mapper.Map<DeviceExtendedDto, DeviceResource>(result);
                return Ok(resource);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<DeviceExtendedDto> Get()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetDeviceAsync(request, cancellationToken: token));
            }
        }

        // GET: api/v1/devices/name/device1?includeLocation=true
        [HttpGet("name/{deviceName}")]
        public async Task<ActionResult<DeviceResource>> GetDeviceByName(string deviceName,
            [FromQuery] DeviceParameters parameters, CancellationToken token)
        {
            var request = new GetDeviceByNameRequest
            {
                DeviceName = deviceName,
                Parameters = new GetRequestParameters
                {
                    UserId = UserId,
                    IncludeFields = { parameters.FieldsToInclude() }
                }
            };
            var cacheKey = $"{nameof(GetDeviceByName)}-{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                DeviceExtendedDto result;
                if (DenyCache)
                {
                    result = await Get();
                    _cache.Add(cacheKey, result, cacheTimespan);
                }
                else
                {
                    result = await _cache.GetOrAddAsync(cacheKey, Get, cacheTimespan);
                }

                var resource = _mapper.Map<DeviceExtendedDto, DeviceResource>(result);
                return Ok(resource);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<DeviceExtendedDto> Get()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetDeviceByNameAsync(request, cancellationToken: token));
            }
        }

        // POST: api/v1/Devices
        [HttpPost]
        public async Task<ActionResult<DeviceResource>> CreateDevice([FromBody] CreateDeviceResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<CreateDeviceResource, CreateDeviceRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdDevice = await _grpcService.SendRequestAsync(async client =>
                    await client.CreateDeviceAsync(request, cancellationToken: token));
                return CreatedAtAction("GetDevice", new { id = createdDevice.Id },
                    _mapper.Map<DeviceExtendedDto, DeviceResource>(createdDevice));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Devices/id/42
        [HttpPut("id/{id:int}")]
        public async Task<ActionResult<DeviceResource>> UpdateDevice(int id, UpdateDeviceResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateDeviceResource, UpdateDeviceRequest>(resource);
            request.Id = id;
            request.UserId = UserId;
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.UpdateDeviceAsync(request, cancellationToken: token));
                return Ok(_mapper.Map<DeviceDto, DeviceResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Devices/id/42/token
        [HttpGet("id/{id:int}/token", Name = "Generate device token")]
        public async Task<ActionResult<GenerateTokenResponse>> GenerateToken(int id, CancellationToken token)
        {
            var request = new GenerateTokenRequest { Id = id, UserId = UserId };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.GenerateTokenAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Devices/id/42
        [HttpDelete("id/{id:int}")]
        public async Task<ActionResult<DeleteDeviceResponse>> DeleteDevice(int id, CancellationToken token)
        {
            var request = new GenericDeleteRequest { Id = id, UserId = UserId };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.DeleteDeviceAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}