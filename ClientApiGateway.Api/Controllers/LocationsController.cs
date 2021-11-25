using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources;
using ClientApiGateway.Api.Resources.Location;
using DeviceManager.Core.Proto;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;
using static ClientApiGateway.Api.Handlers.RpcExceptionHandler;
using LocationResource = DeviceManager.Core.Proto.LocationResource;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<LocationsController> _logger;
        private readonly LocationGrpcService.LocationGrpcServiceClient _locationService;
        private readonly IMapper _mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public LocationsController(
            ILogger<LocationsController> logger,
            LocationGrpcService.LocationGrpcServiceClient locationService,
            IMapper mapper)
        {
            _logger = logger;
            _locationService = locationService;
            _mapper = mapper;
        }

        // GET: api/v1/Locations?includeDevices=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetLocationResource>>> GetAllLocations(
            [FromQuery] bool includeDevices)
        {
            var request = new GetAllLocationsRequest
            {
                IncludeDevices = includeDevices,
                UserId = UserId
            };
            try
            {
                var locations = new List<GetLocationResource>();
                var call = _locationService.GetAllLocations(request);
                await foreach (var location in call.ResponseStream.ReadAllAsync())
                {
                    var locRes = _mapper.Map<LocationResourceExtended, GetLocationResource>(location);
                    if (!includeDevices) locRes.Devices = null;
                    locations.Add(locRes);
                }

                return Ok(locations);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Locations/all?includeDevices=true
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GetLocationResource>>> GetAllLocationsOfAllUsers(
            [FromQuery] bool includeDevices)
        {
            var request = new GetAllLocationsRequest { IncludeDevices = includeDevices };
            try
            {
                var locations = new List<GetLocationResource>();
                var call = _locationService.GetAllLocations(request);
                await foreach (var location in call.ResponseStream.ReadAllAsync())
                {
                    var locRes = _mapper.Map<LocationResourceExtended, GetLocationResource>(location);
                    if (!includeDevices) locRes.Devices = null;
                    locations.Add(locRes);
                }

                return Ok(locations);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Locations/42?includeDevices=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetLocationResource>> GetLocation(int id, [FromQuery] bool includeDevices)
        {
            var request = new GetLocationRequest
            {
                Id = id,
                IncludeDevices = includeDevices,
                UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
            };
            try
            {
                var response = await _locationService.GetLocationAsync(request);
                var location = _mapper.Map<LocationResourceExtended, GetLocationResource>(response);
                if (!includeDevices) location.Devices = null;
                return Ok(location);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Locations
        [HttpPost]
        public async Task<ActionResult<LocationResource>> CreateLocation(CreateLocationResource resource)
        {
            var request = _mapper.Map<CreateLocationResource, CreateLocationRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdLocation = await _locationService.CreateLocationAsync(request);
                return Created($"api/v1/Locations/{createdLocation.Id}", createdLocation);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Locations/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LocationResource>> UpdateLocation(UpdateLocationResource resource, int id)
        {
            var request = _mapper.Map<UpdateLocationResource, UpdateLocationRequest>(resource);
            request.UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId;
            request.Id = id;
            try
            {
                return Ok(await _locationService.UpdateLocationAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Locations/42
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<LocationResource>> DeleteLocation(int id)
        {
            var request = new DeleteLocationRequest
            {
                Id = id,
                UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
            };
            try
            {
                return Ok(await _locationService.DeleteLocationAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}