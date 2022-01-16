using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources;
using DeviceManager.Core.Models;
using Grpc.Core;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Proto;
using Shared.Services.GrpcClientServices;
using UserIdentity.Core.Models.Auth;
using static ClientApiGateway.Api.ExceptionHandlers.RpcExceptionHandler;

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
        private readonly IAppCache _cache;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private bool DenyCache => string.Equals(Request.Headers["Cache-Control"], "no-cache",
            StringComparison.InvariantCultureIgnoreCase);

        public SensorsController(
            ILogger<SensorsController> logger, IMapper mapper,
            IGrpcService<SensorGrpc.SensorGrpcClient> grpcService, IAppCache cache)
        {
            _logger = logger;
            _mapper = mapper;
            _grpcService = grpcService;
            _cache = cache;
        }

        // GET: api/v1/Sensors?detailed=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorResource>>> GetAllSensors(
            [FromQuery] PaginationParameters pagination, [FromQuery] SensorParameters parameters,
            CancellationToken token)
        {
            return await GetSensors(pagination, parameters, true, token);
        }

        // GET: api/v1/Sensors/all?detailed=true
        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SensorResource>>> GetSensorOfAllUsers(
            [FromQuery] PaginationParameters pagination, [FromQuery] SensorParameters parameters,
            CancellationToken token)
        {
            return await GetSensors(pagination, parameters, false, token);
        }

        private async Task<ActionResult<IEnumerable<SensorResource>>> GetSensors(
            [FromQuery] PaginationParameters pagination, [FromQuery] SensorParameters parameters, bool limitToUser,
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
            var cacheKey = $"{nameof(GetSensors)}{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                GetAllSensorsResponse result;
                if (DenyCache)
                {
                    result = await GetAll();
                    _cache.Add(cacheKey, result, cacheTimespan);
                }
                else
                {
                    result = await _cache.GetOrAddAsync(cacheKey, GetAll, cacheTimespan);
                }

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
                var resources = result.Sensors.Select(s => _mapper.Map<SensorDto, SensorResource>(s));
                return Ok(resources);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<GetAllSensorsResponse> GetAll()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetAllSensorsAsync(request, cancellationToken: token));
            }
        }


        // GET: api/v1/Sensors/id/42?detailed=true
        [HttpGet("id/{id:int}")]
        public async Task<ActionResult<SensorResource>> GetSensor(int id, [FromQuery] SensorParameters parameters,
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
            var cacheKey = $"{nameof(GetSensor)}{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                SensorDto result;
                if (DenyCache)
                {
                    result = await Get();
                    _cache.Add(cacheKey, result, cacheTimespan);
                }
                else
                {
                    result = await _cache.GetOrAddAsync(cacheKey, Get, cacheTimespan);
                }

                return Ok(_mapper.Map<SensorDto, SensorResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<SensorDto> Get()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetSensorAsync(request, cancellationToken: token));
            }
        }


        // GET: api/v1/Sensors/device/42/name/temp1?detailed=true
        [HttpGet("name/{sensorName}/device/{deviceId:int}")]
        public async Task<ActionResult<SensorResource>> GetSensorByName(int deviceId, string sensorName,
            [FromQuery] SensorParameters parameters, CancellationToken token)
        {
            var request = new GetSensorByNameRequest
            {
                DeviceId = deviceId,
                SensorName = sensorName,
                Parameters = new GetRequestParameters
                {
                    UserId = User.IsInRole(DefaultRoles.SuperUser) ? null : UserId,
                    IncludeFields = { parameters.FieldsToInclude() }
                }
            };
            var cacheKey = $"{nameof(GetSensorByName)}{request}";
            var cacheTimespan = TimeSpan.FromSeconds(15);
            try
            {
                SensorDto result;
                if (DenyCache)
                {
                    result = await Get();
                    _cache.Add(cacheKey, result, cacheTimespan);
                }
                else
                {
                    result = await _cache.GetOrAddAsync(cacheKey, Get, cacheTimespan);
                }

                return Ok(_mapper.Map<SensorDto, SensorResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }

            async Task<SensorDto> Get()
            {
                return await _grpcService.SendRequestAsync(async client =>
                    await client.GetSensorByNameAsync(request, cancellationToken: token));
            }
        }

        // POST: api/v1/Sensors
        [HttpPost]
        public async Task<ActionResult<SensorResource>> AddSensor(SaveSensorResource resource, CancellationToken token)
        {
            var request = _mapper.Map<SaveSensorResource, CreateSensorRequest>(resource);
            request.UserId = UserId;
            try
            {
                var createdSensor = await _grpcService.SendRequestAsync(async client =>
                    await client.AddSensorAsync(request, cancellationToken: token));
                return Created($"api/v1/Sensors/{createdSensor.Id}",
                    _mapper.Map<SensorDto, SensorResource>(createdSensor));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // PUT: api/v1/Sensors/id/42
        [HttpPut("id/{id:int}")]
        public async Task<ActionResult<SensorResource>> UpdateSensor(SaveSensorResource resource, int id,
            CancellationToken token)
        {
            var request = _mapper.Map<SaveSensorResource, UpdateSensorRequest>(resource);
            request.UserId = UserId;
            try
            {
                var result = await _grpcService.SendRequestAsync(async client =>
                    await client.UpdateSensorAsync(request, cancellationToken: token));
                return Ok(_mapper.Map<SensorDto, SensorResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // DELETE: api/v1/Sensors/id/42
        [HttpDelete("id/{id:int}")]
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