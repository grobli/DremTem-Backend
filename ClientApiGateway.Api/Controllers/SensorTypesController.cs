using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.SensorType;
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
        public async Task<ActionResult<IEnumerable<SensorTypeResource>>> GetAllSensorTypes()
        {
            var request = new GetAllSensorTypesRequest();
            try
            {
                var sensorTypes = new List<SensorTypeResource>();
                var call = _typeService.GetAllSensorTypes(request);
                await foreach (var sensorType in call.ResponseStream.ReadAllAsync())
                {
                    sensorTypes.Add(sensorType);
                }

                return Ok(sensorTypes);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET: api/v1/SensorTypes/42
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SensorTypeResource>> GetSensorType(int id)
        {
            var request = new GetSensorTypeRequest { Id = id };
            try
            {
                return Ok(await _typeService.GetSensorTypeAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST: api/v1/SensorTypes
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost]
        public async Task<ActionResult<SensorTypeResource>> CreateSensorType(CreateSensorTypeRequest request)
        {
            try
            {
                var createdType = await _typeService.CreateSensorTypeAsync(request);
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
        public async Task<ActionResult<SensorTypeResource>> UpdateSensorType(UpdateSensorTypeResource resource, int id)
        {
            var request = _mapper.Map<UpdateSensorTypeResource, UpdateSensorTypeRequest>(resource);
            request.Id = id;
            try
            {
                return Ok(await _typeService.UpdateSensorTypeAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/SensorTypes/42
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<SensorTypeResource>> DeleteSensorType(int id)
        {
            var request = new DeleteSensorTypeRequest { Id = id };
            try
            {
                return Ok(await _typeService.DeleteSensorTypeAsync(request));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}