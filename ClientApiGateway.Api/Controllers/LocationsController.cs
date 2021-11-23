using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientApiGateway.Api.Resources;
using DeviceManager.Core.Proto;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<LocationsController> _logger;
        private readonly LocationGrpcService.LocationGrpcServiceClient _locationService;

        public LocationsController(
            ILogger<LocationsController> logger,
            LocationGrpcService.LocationGrpcServiceClient locationService)
        {
            _logger = logger;
            _locationService = locationService;
        }

        // GET: api/v1/Locations?includeDevices=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationResource>>> GetAllLocations([FromQuery] bool includeDevices)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = new GetAllLocationsRequest
            {
                IncludeDevices = includeDevices,
                UserId = userId
            };

            try
            {
                var locations = new List<LocationResource>();
                var call = _locationService.GetAllLocations(request);
                await foreach (var location in call.ResponseStream.ReadAllAsync())
                {
                    locations.Add(location);
                }

                return Ok(locations);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Locations/all?includeDevices=true
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<LocationResource>>> GetAllLocationsOfAllUsers(
            [FromQuery] bool includeDevices)
        {
            var request = new GetAllLocationsRequest { IncludeDevices = includeDevices };

            try
            {
                var locations = new List<LocationResource>();
                var call = _locationService.GetAllLocations(request);
                await foreach (var location in call.ResponseStream.ReadAllAsync())
                {
                    locations.Add(location);
                }

                return Ok(locations);
            }
            catch (RpcException e)
            {
                return BadRequest(e.Status);
            }
        }

        // GET: api/v1/Locations/kitchen
        [HttpGet("{name}")]
        public async Task<ActionResult<LocationResource>> GetLocation(string name)
        {
            throw new NotImplementedException();
        }

        // GET: api/v1/Locations/kitchen/fa87b04b-002f-4490-9c4d-659c474924cd
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("{name}/{userId:guid}")]
        public async Task<ActionResult<LocationResource>> GetLocation(string name, Guid userId)
        {
            throw new NotImplementedException();
        }

        // POST: api/v1/Locations
        [HttpPost]
        public async Task<ActionResult<LocationResource>> CreateLocation(CreateLocationResource resource, string name)
        {
            throw new NotImplementedException();
        }

        // PUT: api/v1/Locations/kitchen
        [HttpPut("{name}")]
        public async Task<ActionResult<LocationResource>> UpdateLocation(UpdateLocationResource resource, string name)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/v1/Locations/kitchen
        [HttpDelete("{name}")]
        public async Task<ActionResult<LocationResource>> DeleteLocation(string name)
        {
            throw new NotImplementedException();
        }
    }
}