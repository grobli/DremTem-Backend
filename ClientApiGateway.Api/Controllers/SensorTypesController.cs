using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.SensorType;
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
    public class SensorTypesController : ControllerBase
    {
        private readonly ILogger<SensorTypesController> _logger;
        private readonly SensorTypeGrpcService.SensorTypeGrpcServiceClient _typeService;
        private readonly IMapper _mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public SensorTypesController(
            ILogger<SensorTypesController> logger,
            SensorTypeGrpcService.SensorTypeGrpcServiceClient typeService,
            IMapper mapper)
        {
            _logger = logger;
            _typeService = typeService;
            _mapper = mapper;
        }

        // GET: api/v1/SensorTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorTypeDto>>> GetAllSensorTypes(
            [FromQuery] SensorTypePagedParameters parameters, CancellationToken token)
        {
            var request = new GenericGetManyRequest
            {
                Parameters = new GetRequestParameters
                {
                    IncludeFields = { parameters.FieldsToInclude() }
                },
                PageNumber = parameters.Page.Number,
                PageSize = parameters.Page.Size
            };
            try
            {
                var result = await _typeService.GetAllSensorTypesAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                return Ok(result.SensorTypes);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/SensorTypes/42
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SensorTypeDto>> GetSensorType(int id,
            [FromQuery] SensorTypeParameters parameters, CancellationToken token)
        {
            var request = new GenericGetRequest
            {
                Id = id
            };
            try
            {
                return Ok(await _typeService.GetSensorTypeAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/SensorTypes
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost]
        public async Task<ActionResult<SensorTypeDto>> CreateSensorType(CreateSensorTypeRequest request,
            CancellationToken token)
        {
            try
            {
                var createdType = await _typeService.CreateSensorTypeAsync(request, cancellationToken: token);
                return Created($"api/v1/Sensors/{createdType.Id}", createdType);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/SensorTypes/42
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SensorTypeDto>> UpdateSensorType(UpdateSensorTypeResource resource, int id,
            CancellationToken token)
        {
            var request = _mapper.Map<UpdateSensorTypeResource, UpdateSensorTypeRequest>(resource);
            request.Id = id;
            try
            {
                return Ok(await _typeService.UpdateSensorTypeAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/SensorTypes/42
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<SensorTypeDto>> DeleteSensorType(int id, CancellationToken token)
        {
            var request = new GenericDeleteRequest { Id = id, UserId = UserId };
            try
            {
                return Ok(await _typeService.DeleteSensorTypeAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}