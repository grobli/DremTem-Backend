using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources;
using DeviceManager.Core.Proto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            throw new NotImplementedException();
        }

        // GET: api/v1/Sensors/all?detailed=true
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SensorResource>>> GetSensorOfAllUsers([FromQuery] bool detailed)
        {
            throw new NotImplementedException();
        }

        // GET: api/v1/Sensors/42?detailed=true
        [HttpGet("{id:long}")]
        public async Task<ActionResult<SensorResource>> GetSensor(long id,
            [FromQuery] bool detailed)
        {
            throw new NotImplementedException();
        }

        // POST: api/v1/Sensors
        [HttpPost]
        public async Task<ActionResult<SensorResource>> AddSensor(SaveSensorResource resource)
        {
            throw new NotImplementedException();
        }

        // PUT: api/v1/Sensors/42
        [HttpPut("{id:long}")]
        public async Task<ActionResult<SensorResource>> UpdateSensor(SaveSensorResource resource, long id)
        {
            throw new NotImplementedException();
        }
    }
}