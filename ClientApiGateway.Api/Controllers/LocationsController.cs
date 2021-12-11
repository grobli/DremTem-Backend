using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.Location;
using DeviceManager.Core.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Location;
using Shared.Services.GrpcClientServices;
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
        private readonly IGrpcService<LocationGrpc.LocationGrpcClient> _grpcService;
        private readonly IMapper _mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public LocationsController(
            ILogger<LocationsController> logger,
            IMapper mapper, IGrpcService<LocationGrpc.LocationGrpcClient> grpcService)
        {
            _logger = logger;
            _mapper = mapper;
            _grpcService = grpcService;
        }

        // GET: api/v1/Locations?includeDevices=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationResource>>> GetAllLocations(
            [FromQuery] PaginationParameters pagination, [FromQuery] LocationParameters parameters,
            CancellationToken token)
        {
            return await GetAllLocations(pagination, parameters, true, token);
        }

        // GET: api/v1/Locations/all?includeDevices=true
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<LocationResource>>> GetAllLocationsOfAllUsers(
            [FromQuery] PaginationParameters pagination, [FromQuery] LocationParameters parameters,
            CancellationToken token)
        {
            return await GetAllLocations(pagination, parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<LocationResource>>> GetAllLocations(
            [FromQuery] PaginationParameters pagination, [FromQuery] LocationParameters parameters, bool limitToUser,
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
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.GetAllLocationsAsync(request, cancellationToken: token));
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                var resources = result.Locations
                    .Select(l => _mapper.Map<LocationExtendedDto, LocationResource>(l));
                return Ok(resources);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Locations/42?includeDevices=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LocationResource>> GetLocation(int id,
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
                var response = await _grpcService.SendRequestAsync(async client =>
                    await client.GetLocationAsync(request, cancellationToken: token));
                var location = _mapper.Map<LocationExtendedDto, LocationResource>(response);
                return Ok(location);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Locations
        [HttpPost]
        public async Task<ActionResult<LocationResource>> CreateLocation(CreateLocationResource resource,
            CancellationToken token)
        {
            var request = _mapper.Map<CreateLocationResource, CreateLocationRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdLocation = await _grpcService.SendRequestAsync(async client =>
                    await client.CreateLocationAsync(request, cancellationToken: token));
                var createdResource = _mapper.Map<LocationDto, LocationResource>(createdLocation);
                return CreatedAtAction("GetLocation", new { id = createdLocation.Id }, createdResource);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Locations/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LocationResource>> UpdateLocation(UpdateLocationResource resource, int id,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateLocationResource, UpdateLocationRequest>(resource);
            request.UserId = UserId;
            request.Id = id;
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.UpdateLocationAsync(request, cancellationToken: token));
                var updatedResource = _mapper.Map<LocationDto, LocationResource>(result);
                return Ok(updatedResource);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Locations/42
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Empty>> DeleteLocation(int id, CancellationToken token)
        {
            var request = new GenericDeleteRequest { Id = id, UserId = UserId };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.DeleteLocationAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}