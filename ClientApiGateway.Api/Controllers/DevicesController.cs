using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources;
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

        // GET: api/v1/Devices?includeLocation=true&includeSensors=false
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResourceExtended>>> GetAllDevices(
            [FromQuery] bool includeLocation,
            [FromQuery] bool includeSensors)
        {
            var request = new GetAllDevicesRequest
            {
                UserId = UserId,
                IncludeLocation = includeLocation,
                IncludeSensors = includeSensors
            };

            try
            {
                var devices = new List<DeviceResourceExtended>();
                var call = _deviceService.GetAllDevices(request);
                await foreach (var device in call.ResponseStream.ReadAllAsync())
                {
                    devices.Add(device);
                }

                return Ok(devices);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Devices/all?includeLocation=true&includeSensors=false
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DeviceResourceExtended>>> GetAllDevicesOfAllUsers(
            [FromQuery] bool includeLocation,
            [FromQuery] bool includeSensors)
        {
            var request = new GetAllDevicesRequest
            {
                IncludeLocation = includeLocation,
                IncludeSensors = includeSensors
            };

            try
            {
                var devices = new List<DeviceResourceExtended>();
                var call = _deviceService.GetAllDevices(request);
                await foreach (var device in call.ResponseStream.ReadAllAsync())
                {
                    devices.Add(device);
                }

                return Ok(devices);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Devices/42?includeLocation=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeviceResourceExtended>> GetDevice(
            int id,
            [FromQuery] bool includeLocation,
            [FromQuery] bool includeSensors)
        {
            var request = new GetDeviceRequest
            {
                Id = id,
                IncludeLocation = includeLocation,
                IncludeSensors = includeSensors,
                UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
            };

            try
            {
                return Ok(await _deviceService.GetDeviceAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Devices
        [HttpPost]
        public async Task<ActionResult<DeviceResource>> CreateDevice(CreateDeviceResource resource)
        {
            var request = _mapper.Map<CreateDeviceResource, CreateDeviceRequest>(resource);
            request.UserId = UserId;

            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(request);
                return Created($"api/v1/Devices/{createdDevice.Id}", createdDevice);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Devices/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<DeviceResource>> UpdateDevice(int id, UpdateDeviceResource resource)
        {
            var request = _mapper.Map<UpdateDeviceResource, UpdateDeviceRequest>(resource);
            request.Id = id;
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                return Ok(await _deviceService.UpdateDeviceAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Devices/42/token
        [HttpGet("{id:int}/token", Name = "Generate device token")]
        public async Task<ActionResult<GenerateTokenResponse>> GenerateToken(int id)
        {
            var request = new GenerateTokenRequest
            {
                Id = id,
                UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
            };

            try
            {
                return Ok(await _deviceService.GenerateTokenAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}