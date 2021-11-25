using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.Sensor;
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
        public async Task<ActionResult<IEnumerable<SensorResource>>> GetAllSensors([FromQuery] bool detailed)
        {
            var request = new GetAllSensorsRequest
            {
                UserId = UserId,
                IncludeType = detailed
            };
            try
            {
                var sensors = new List<SensorResource>();
                var call = _sensorService.GetAllSensors(request);
                await foreach (var sensor in call.ResponseStream.ReadAllAsync())
                {
                    sensors.Add(sensor);
                }

                return Ok(sensors);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Sensors/all?detailed=true
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SensorResource>>> GetSensorOfAllUsers([FromQuery] bool detailed)
        {
            var request = new GetAllSensorsRequest { IncludeType = detailed };
            try
            {
                var sensors = new List<SensorResource>();
                var call = _sensorService.GetAllSensors(request);
                await foreach (var sensor in call.ResponseStream.ReadAllAsync())
                {
                    sensors.Add(sensor);
                }

                return Ok(sensors);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/Sensors/42?detailed=true
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SensorResource>> GetSensor(int id, [FromQuery] bool detailed)
        {
            var request = new GetSensorRequest
            {
                Id = id,
                IncludeType = detailed,
                UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId
            };
            try
            {
                return Ok(await _sensorService.GetSensorAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/Sensors
        [HttpPost]
        public async Task<ActionResult<SensorResource>> AddSensor(SaveSensorResource resource)
        {
            var request = _mapper.Map<SaveSensorResource, CreateSensorRequest>(resource);
            request.UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId;
            try
            {
                var createdSensor = await _sensorService.AddSensorAsync(request);
                return Created($"api/v1/Sensors/{createdSensor.Id}", createdSensor);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Sensors/42
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SensorResource>> UpdateSensor(SaveSensorResource resource, int id)
        {
            var request = _mapper.Map<SaveSensorResource, UpdateSensorRequest>(resource);
            request.UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId;
            try
            {
                return Ok(await _sensorService.UpdateSensorAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}