using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.Sensor;
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
    public class SensorsController : ControllerBase
    {
        private readonly ILogger<SensorsController> _logger;
        private readonly SensorGrpcService.SensorGrpcServiceClient _sensorService;
        private readonly IMapper _mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public SensorsController(
            ILogger<SensorsController> logger,
            SensorGrpcService.SensorGrpcServiceClient sensorService,
            IMapper mapper)
        {
            _logger = logger;
            _sensorService = sensorService;
            _mapper = mapper;
        }

        // GET: api/v1/Sensors?detailed=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetAllSensors(
            [FromQuery] SensorPagedParameters parameters, CancellationToken token)
        {
            return await GetSensors(parameters, true, token);
        }

        // GET: api/v1/Sensors/all?detailed=true
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetSensorOfAllUsers(
            [FromQuery] SensorPagedParameters parameters, CancellationToken token)
        {
            return await GetSensors(parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<SensorDto>>> GetSensors(
            [FromQuery] SensorPagedParameters parameters, bool limitToUser, CancellationToken token)
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
                var result = await _sensorService.GetAllSensorsAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                return Ok(result.Sensors);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Sensors/42?detailed=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SensorDto>> GetSensor(int id, [FromQuery] SensorParameters parameters,
            CancellationToken token)
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
                return Ok(await _sensorService.GetSensorAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Sensors
        [HttpPost]
        public async Task<ActionResult<SensorDto>> AddSensor(SaveSensorResource resource, CancellationToken token)
        {
            var request = _mapper.Map<SaveSensorResource, CreateSensorRequest>(resource);
            request.UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId;
            try
            {
                var createdSensor = await _sensorService.AddSensorAsync(request, cancellationToken: token);
                return Created($"api/v1/Sensors/{createdSensor.Id}", createdSensor);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Sensors/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SensorDto>> UpdateSensor(SaveSensorResource resource, int id,
            CancellationToken token)
        {
            var request = _mapper.Map<SaveSensorResource, UpdateSensorRequest>(resource);
            request.UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId;
            try
            {
                return Ok(await _sensorService.UpdateSensorAsync(request, cancellationToken: token));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}