using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceManager.Core.Proto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class SensorTypesController : ControllerBase
    {
        private readonly ILogger<SensorTypesController> _logger;
        private readonly SensorTypeGrpcService.SensorTypeGrpcServiceClient _typeService;

        public SensorTypesController(
            ILogger<SensorTypesController> logger,
            SensorTypeGrpcService.SensorTypeGrpcServiceClient typeService)
        {
            _logger = logger;
            _typeService = typeService;
        }

        // GET: api/v1/SensorTypes
        [HttpGet]
        public async Task<IEnumerable<SensorTypeResource>> GetAllSensorTypes()
        {
            throw new NotImplementedException();
        }

        // GET: api/v1/SensorTypes/TemperatureCelsius
        [HttpGet("{typeName}")]
        public async Task<SensorTypeResource> GetSensorType(string typeName)
        {
            throw new NotImplementedException();
        }

        // POST: api/v1/SensorTypes
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost]
        public async Task<SensorTypeResource> CreateSensorType()
        {
            throw new NotImplementedException();
        }

        // PUT: api/v1/SensorTypes/TemperatureCelsius
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPut("{typeName}")]
        public async Task<SensorTypeResource> UpdateSensorType(string typeName)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/v1/SensorTypes/TemperatureCelsius
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpDelete("{typeName}")]
        public async Task<SensorTypeResource> DeleteSensorType(string typeName)
        {
            throw new NotImplementedException();
        }
    }
}