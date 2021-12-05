using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources.Sensor;
using DeviceManager.Core.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Proto;
using Shared.Proto.Common;
using Shared.Proto.Sensor;
using Shared.Services.GrpcClientServices;
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
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _grpcService;
        private readonly IMapper _mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public SensorsController(
            ILogger<SensorsController> logger, IMapper mapper,
            IGrpcService<SensorGrpc.SensorGrpcClient> grpcService)
        {
            _logger = logger;
            _mapper = mapper;
            _grpcService = grpcService;
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
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.GetAllSensorsAsync(request, cancellationToken: token));
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
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.GetSensorAsync(request, cancellationToken: token));
                return Ok(result);
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
                var createdSensor = await _grpcService.SendRequestAsync(async client =>
                    await client.AddSensorAsync(request, cancellationToken: token));
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
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.UpdateSensorAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Sensors/42
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<DeleteSensorResponse>> DeleteSensor(int id, CancellationToken token)
        {
            var request = new GenericDeleteRequest { Id = id, UserId = UserId };
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.DeleteSensorAsync(request, cancellationToken: token));
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}