using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private readonly DeviceGrpcService.DeviceGrpcServiceClient _deviceService;
        private readonly IMapper _mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public DevicesController(
            ILogger<DevicesController> logger,
            DeviceGrpcService.DeviceGrpcServiceClient deviceService,
            IMapper mapper)
        {
            _logger = logger;
            _deviceService = deviceService;
            _mapper = mapper;
        }

        // GET: api/v1/Devices?pageNumber=1&pageSize=3&includeLocation=true&includeSensors=false
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResourceExtended>>> GetAllDevices(
            [FromQuery] DevicePagedParameters parameters, CancellationToken token)
        {
            return await GetAllDevices(parameters, true, token);
        }

        // GET: api/v1/Devices/all?includeLocation=true&includeSensors=false
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DeviceResourceExtended>>> GetAllDevicesOfAllUsers(
            [FromQuery] DevicePagedParameters parameters, CancellationToken token)
        {
            return await GetAllDevices(parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<DeviceResourceExtended>>> GetAllDevices(
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
                var result = await _deviceService.GetAllDevicesAsync(request, cancellationToken: token);
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
        public async Task<ActionResult<DeviceResourceExtended>> GetDevice(int id,
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
                return Ok(await _deviceService.GetDeviceAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Devices
        [HttpPost]
        public async Task<ActionResult<DeviceResource>> CreateDevice(CreateDeviceResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<CreateDeviceResource, CreateDeviceRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(request, cancellationToken: token);
                return Created($"api/v1/Devices/{createdDevice.Id}", createdDevice);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Devices/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<DeviceResource>> UpdateDevice(int id, UpdateDeviceResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateDeviceResource, UpdateDeviceRequest>(resource);
            request.Id = id;
            request.UserId = UserId;
            try
            {
                return Ok(await _deviceService.UpdateDeviceAsync(request, cancellationToken: token));
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
            var request = new GenerateTokenRequest
            {
                Id = id,
                UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
            };
            try
            {
                return Ok(await _deviceService.GenerateTokenAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}