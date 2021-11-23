using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientApiGateway.Api.Resources;
using DeviceManager.Core.Proto;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;
        private readonly DeviceGrpcService.DeviceGrpcServiceClient _deviceService;

        public DevicesController(
            ILogger<DevicesController> logger,
            DeviceGrpcService.DeviceGrpcServiceClient deviceService)
        {
            _logger = logger;
            _deviceService = deviceService;
        }

        // GET: api/v1/Devices?includeLocation=true&includeSensors=false
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceResource>>> GetAllDevices(
            [FromQuery] bool includeLocation,
            [FromQuery] bool includeSensors)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = new GetAllDevicesRequest
            {
                UserId = userId,
                IncludeLocation = includeLocation,
                IncludeSensors = includeSensors
            };

            try
            {
                var devices = new List<DeviceResource>();
                var call = _deviceService.GetAllDevices(request);
                await foreach (var device in call.ResponseStream.ReadAllAsync())
                {
                    devices.Add(device);
                }

                return Ok(devices);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Devices/all?includeLocation=true&includeSensors=false
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DeviceResource>>> GetAllDevicesOfAllUsers(
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
                var devices = new List<DeviceResource>();
                var call = _deviceService.GetAllDevices(request);
                await foreach (var device in call.ResponseStream.ReadAllAsync())
                {
                    devices.Add(device);
                }

                return Ok(devices);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Devices/42?includeLocation=true
        [HttpGet("{id:long}")]
        public async Task<ActionResult<DeviceResource>> GetDevice(
            long id,
            [FromQuery] bool includeLocation,
            [FromQuery] bool includeSensors)
        {
            var request = new GetDeviceRequest
            {
                Id = id,
                IncludeLocation = includeLocation,
                IncludeSensors = includeSensors,
                UserId = User.IsInRole(DefaultRoles.SuperUser)
                    ? null
                    : User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            try
            {
                return Ok(await _deviceService.GetDeviceAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // POST: api/v1/Devices
        [HttpPost]
        public async Task<ActionResult<DeviceResource>> CreateDevice(CreateDeviceResource resource)
        {
            var request = new CreateDeviceRequest
            {
                Name = resource.Name,
                DisplayName = resource.DisplayName,
                Online = resource.Online,
                LocationName = resource.LocationName,
                Sensors = { resource.Sensors },
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(request);
                return Created($"{Url.Action("GetDevice")}/{createdDevice.Id}", createdDevice);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // PUT: api/v1/Devices/42
        [HttpPut("{id:long}")]
        public async Task<ActionResult<DeviceResource>> UpdateDevice(long id, UpdateDeviceResource resource)
        {
            var (displayName, online, locationName) = resource;
            var request = new UpdateDeviceRequest()
            {
                Id = id,
                DisplayName = displayName,
                Online = online,
                LocationName = locationName,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            try
            {
                return Ok(await _deviceService.UpdateDeviceAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Devices/42/token
        [HttpGet("{id:long}/token", Name = "Generate device token")]
        public async Task<ActionResult<GenerateTokenResponse>> GenerateToken(long id)
        {
            var request = new GenerateTokenRequest
            {
                Id = id,
                UserId = User.IsInRole(DefaultRoles.SuperUser)
                    ? null
                    : User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            try
            {
                return Ok(await _deviceService.GenerateTokenAsync(request));
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }
    }
}