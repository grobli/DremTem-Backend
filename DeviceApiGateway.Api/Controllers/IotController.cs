using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DeviceApiGateway.Api.Resources;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Proto.Device;
using Shared.Proto.SensorData;
using Shared.Services.GrpcClientServices;
using static DeviceApiGateway.Api.Handlers.RpcExceptionHandler;

namespace DeviceApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly ILogger<IotController> _logger;
        private readonly IGrpcService<DeviceGrpc.DeviceGrpcClient> _deviceService;
        private readonly IGrpcService<SensorDataGrpc.SensorDataGrpcClient> _dataService;

        private int DeviceId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IotController(ILogger<IotController> logger, IGrpcService<DeviceGrpc.DeviceGrpcClient> deviceService,
            IGrpcService<SensorDataGrpc.SensorDataGrpcClient> dataService)
        {
            _logger = logger;
            _deviceService = deviceService;
            _dataService = dataService;
        }

        // POST: api/v1/iot/readings/send/sensor/temp1
        [HttpPost("readings/send/sensor/{sensorName}")]
        public async Task<IActionResult> SendReadingToServer(string sensorName,
            [FromBody] SaveReadingResource resource, [FromQuery] bool allowOverwrite, CancellationToken token)
        {
            var (timestamp, value) = resource;
            var request = new CreateReadingRequest
            {
                Time = Timestamp.FromDateTime(DateTime.Parse(timestamp).ToUniversalTime()),
                Value = value,
                AllowOverwrite = allowOverwrite,
                DeviceAndName = new DeviceAndSensorName { DeviceId = DeviceId, SensorName = sensorName }
            };

            var tasks = new List<Task>
            {
                _deviceService.SendRequestAsync(async client =>
                    await client.PingAsync(new PingRequest { Id = DeviceId }, cancellationToken: token)),
                _dataService.SendRequestAsync(async client =>
                    await client.CreateReadingAsync(request, cancellationToken: token))
            };

            try
            {
                await Task.WhenAll(tasks);
                return Ok();
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // POST  api/v1/iot/ping
        [HttpPost("ping")]
        public async Task<IActionResult> Ping(CancellationToken token)
        {
            var request = new PingRequest { Id = DeviceId };
            try
            {
                await _deviceService.SendRequestAsync(async client =>
                    await client.PingAsync(request, cancellationToken: token));
                return Ok();
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }
    }
}