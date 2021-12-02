using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.Device;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
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
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;

        //     private readonly DeviceGrpcService.DeviceGrpcServiceClient _deviceService;
        private readonly IMapper _mapper;
        private readonly IGrpcClient<DeviceGrpcService.DeviceGrpcServiceClient> _client;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public DevicesController(
            ILogger<DevicesController> logger, IMapper mapper,
            IGrpcClient<DeviceGrpcService.DeviceGrpcServiceClient> client)
        {
            _logger = logger;
            _mapper = mapper;
            _client = client;
        }

        // GET: api/v1/Devices?pageNumber=1&pageSize=3&includeLocation=true&includeSensors=false
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceExtendedDto>>> GetAllDevices(
            [FromQuery] DevicePagedParameters parameters, CancellationToken token)
        {
            return await GetAllDevices(parameters, true, token);
        }

        // GET: api/v1/Devices/all?includeLocation=true&includeSensors=false
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DeviceExtendedDto>>> GetAllDevicesOfAllUsers(
            [FromQuery] DevicePagedParameters parameters, CancellationToken token)
        {
            return await GetAllDevices(parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<DeviceExtendedDto>>> GetAllDevices(
            [FromQuery] DevicePagedParameters parameters, bool limitToUser, CancellationToken token)
        {
            var request = new GenericGetManyRequest
            {
                Parameters = new GetRequestParameters
                {
                    UserId = limitToUser ? UserId : null,
                    IncludeFields = { parameters.FieldsToInclude() }
                },
                PageNumber = parameters.Page.Number,
                PageSize = parameters.Page.Size
            };
            try
            {
                var result = await _client.SendRequestAsync(async client =>
                    await client.GetAllDevicesAsync(request, cancellationToken: token));
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                return Ok(result.Devices);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Devices/42?includeLocation=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeviceExtendedDto>> GetDevice(int id,
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
            try
            {
                var result = await _client.SendRequestAsync(async client =>
                    await client.GetDeviceAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Devices
        [HttpPost]
        public async Task<ActionResult<DeviceDto>> CreateDevice([FromBody] CreateDeviceResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<CreateDeviceResource, CreateDeviceRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdDevice = await _client.SendRequestAsync(async client =>
                    await client.CreateDeviceAsync(request, cancellationToken: token));
                return CreatedAtAction("GetDevice", new { id = createdDevice.Id },
                    createdDevice);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Devices/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<DeviceDto>> UpdateDevice(int id, UpdateDeviceResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateDeviceResource, UpdateDeviceRequest>(resource);
            request.Id = id;
            request.UserId = UserId;
            try
            {
                var result = await _client.SendRequestAsync(async client =>
                    await client.UpdateDeviceAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Devices/42/token
        [HttpGet("{id:int}/token", Name = "Generate device token")]
        public async Task<ActionResult<GenerateTokenResponse>> GenerateToken(int id, CancellationToken token)
        {
            var request = new GenerateTokenRequest { Id = id, UserId = UserId };
            try
            {
                var result = await _client.SendRequestAsync(async client =>
                    await client.GenerateTokenAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Devices/42
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<DeviceDto>> DeleteDevice(int id, CancellationToken token)
        {
            var request = new GenericDeleteRequest { Id = id, UserId = UserId };
            try
            {
                var result = await _client.SendRequestAsync(async client =>
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