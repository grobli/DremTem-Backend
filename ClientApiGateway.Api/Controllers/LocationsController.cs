using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.Location;
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
            [FromQuery] LocationPagedParameters parameters, CancellationToken token)
        {
            return await GetAllLocations(parameters, true, token);
        }

        // GET: api/v1/Locations/all?includeDevices=true
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GetLocationResource>>> GetAllLocationsOfAllUsers(
            [FromQuery] LocationPagedParameters parameters, CancellationToken token)
        {
            return await GetAllLocations(parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<GetLocationResource>>> GetAllLocations(
            [FromQuery] LocationPagedParameters parameters, bool limitToUser, CancellationToken token)
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
                var result = await _locationService.GetAllLocationsAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                return Ok(result.Locations);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Locations/42?includeDevices=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetLocationResource>> GetLocation(int id,
            [FromQuery] LocationParameters parameters, CancellationToken token)
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
                var response = await _locationService.GetLocationAsync(request, cancellationToken: token);
                var location = _mapper.Map<LocationDtoExtended, GetLocationResource>(response);
                if (parameters.IncludeDevices) location.Devices = null;
                return Ok(location);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Locations
        [HttpPost]
        public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<CreateLocationResource, CreateLocationRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdLocation = await _locationService.CreateLocationAsync(request, cancellationToken: token);
                return Created($"api/v1/Locations/{createdLocation.Id}", createdLocation);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Locations/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(UpdateLocationResource resource, int id,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateLocationResource, UpdateLocationRequest>(resource);
            request.UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId;
            request.Id = id;
            try
            {
                return Ok(await _locationService.UpdateLocationAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Locations/42
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<LocationDto>> DeleteLocation(int id, CancellationToken token)
        {
            var request = new GenericDeleteRequest { Id = id, UserId = UserId };
            try
            {
                return Ok(await _locationService.DeleteLocationAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}