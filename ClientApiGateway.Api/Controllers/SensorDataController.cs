using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ClientApiGateway.Api.Resources;
using ClientApiGateway.Api.Resources.Reading;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SensorData.Core.Models;
using Shared.Extensions;
using Shared.Proto.SensorData;
using Shared.Services.GrpcClientServices;
using static ClientApiGateway.Api.Handlers.RpcExceptionHandler;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class SensorDataController : ControllerBase
    {
        private const int ChunkSize = 2 * 1024 * 1024;

        private readonly ILogger<SensorDataController> _logger;
        private readonly IGrpcService<SensorDataGrpc.SensorDataGrpcClient> _dataService;
        private readonly IMapper _mapper;
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);


        public SensorDataController(ILogger<SensorDataController> logger,
            IGrpcService<SensorDataGrpc.SensorDataGrpcClient> dataService, IMapper mapper)
        {
            _logger = logger;
            _dataService = dataService;
            _mapper = mapper;
        }

        // GET api/v1/SensorData/sensor/42/all
        [HttpGet("sensor/{sensorId:int}/all")]
        public async Task<ActionResult<GetReadingsFromSensorResource>> GetAllReadings(int sensorId,
            [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            var request = new GetAllFromSensorRequest
            {
                PageNumber = parameters.Page.Number,
                PageSize = parameters.Page.Size,
                SensorId = sensorId
            };

            try
            {
                var client = _dataService.GetClient(UserId);
                var result = await client.GetAllFromSensorAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.PaginationMetaData));
                return Ok(_mapper.Map<GetManyFromSensorResponse, GetReadingsFromSensorResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET api/v1/SensorData/device/42/sensor/temp1/all
        [HttpGet("device/{deviceId:int}/sensor/{sensorName}/all")]
        public async Task<ActionResult<GetReadingsFromSensorResource>> GetAllReadings(int deviceId, string sensorName,
            [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            var request = new GetAllFromSensorRequest
            {
                PageNumber = parameters.Page.Number,
                PageSize = parameters.Page.Size,
                DeviceAndName = new DeviceAndSensorName { DeviceId = deviceId, SensorName = sensorName }
            };

            try
            {
                var client = _dataService.GetClient(UserId);
                var result = await client.GetAllFromSensorAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.PaginationMetaData));
                return Ok(_mapper.Map<GetManyFromSensorResponse, GetReadingsFromSensorResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET api/v1/SensorData/device/42/sensor/temp1/last/second/40
        [HttpGet("device/{deviceId:int}/sensor/{sensorName}/last/second")]
        public async Task<ActionResult<GetReadingsFromSensorResource>> GetLastReadingsBySeconds(int deviceId,
            string sensorName,
            [FromQuery] int amount, [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            return await GetLastReadings(deviceId, sensorName, TimeUnit.Second, amount <= 0 ? 1 : amount, parameters,
                token);
        }

        // GET api/v1/SensorData/device/42/sensor/temp1/last/minute/40
        [HttpGet("device/{deviceId:int}/sensor/{sensorName}/last/minute")]
        public async Task<ActionResult<GetReadingsFromSensorResource>> GetLastReadingsByMinutes(int deviceId,
            string sensorName,
            [FromQuery] int amount, [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            return await GetLastReadings(deviceId, sensorName, TimeUnit.Minute, amount <= 0 ? 1 : amount, parameters,
                token);
        }


        // GET api/v1/SensorData/device/42/sensor/temp1/last/hour/2
        [HttpGet("device/{deviceId:int}/sensor/{sensorName}/last/hour")]
        public async Task<ActionResult<GetReadingsFromSensorResource>> GetLastReadingsByHours(int deviceId,
            string sensorName,
            [FromQuery] int amount, [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            return await GetLastReadings(deviceId, sensorName, TimeUnit.Hour, amount <= 0 ? 1 : amount, parameters,
                token);
        }

        // GET api/v1/SensorData/device/42/sensor/temp1/last/day/2
        [HttpGet("device/{deviceId:int}/sensor/{sensorName}/last/day")]
        public async Task<ActionResult<GetReadingsFromSensorResource>> GetLastReadingsByDays(int deviceId,
            string sensorName,
            [FromQuery] int amount, [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            return await GetLastReadings(deviceId, sensorName, TimeUnit.Day, amount <= 0 ? 1 : amount, parameters,
                token);
        }


        private async Task<ActionResult<GetReadingsFromSensorResource>> GetLastReadings(int deviceId, string sensorName,
            TimeUnit timeUnit,
            int amount, [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            var request = new GetLastFromSensorRequest
            {
                PageNumber = parameters.Page.Number,
                PageSize = parameters.Page.Size,
                DeviceAndName = new DeviceAndSensorName { DeviceId = deviceId, SensorName = sensorName },
                TimeUnit = timeUnit,
                TimeUnitValue = amount
            };

            try
            {
                var client = _dataService.GetClient(UserId);
                var result = await client.GetLastFromSensorAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.PaginationMetaData));
                return Ok(_mapper.Map<GetManyFromSensorResponse, GetReadingsFromSensorResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET api/v1/SensorData/device/42/sensor/temp1/range/{startDate:datetime}/{endDate:datetime}
        [HttpGet("device/{deviceId:int}/sensor/{sensorName}/range/{startDate:datetime}/{endDate:datetime}")]
        public async Task<ActionResult<GetReadingsFromSensorResource>> GetAllReadings(int deviceId, string sensorName,
            DateTime startDate,
            DateTime endDate, [FromQuery] ReadingPagedParameters parameters, CancellationToken token)
        {
            var request = new GetRangeFromSensorRequest
            {
                PageNumber = parameters.Page.Number,
                PageSize = parameters.Page.Size,
                DeviceAndName = new DeviceAndSensorName { DeviceId = deviceId, SensorName = sensorName },
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(endDate.ToUniversalTime())
            };

            try
            {
                var client = _dataService.GetClient(UserId);
                var result = await client.GetRangeFromSensorAsync(request, cancellationToken: token);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.PaginationMetaData));
                return Ok(_mapper.Map<GetManyFromSensorResponse, GetReadingsFromSensorResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        // GET api/v1/SensorData/device/42/sensor/temp1/latest
        [HttpGet("device/{deviceId:int}/sensor/{sensorName}/latest")]
        public async Task<ActionResult<ReadingResource>> GetLatestReading(int deviceId, string sensorName,
            CancellationToken token)
        {
            var request = new GetFirstRecentFromSensorRequest
                { DeviceAndName = new DeviceAndSensorName { DeviceId = deviceId, SensorName = sensorName } };
            try
            {
                var client = _dataService.GetClient(UserId);
                var result = await client.GetFirstRecentFromSensorAsync(request, cancellationToken: token);
                return Ok(_mapper.Map<ReadingDto, ReadingResource>(result));
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }


        // POST api/v1/SensorData/upload/csv/device/42/sensor/temp1
        [RequestSizeLimit(50L * 1024L * 1024L)] // 50 MiB
        [HttpPost("upload/csv/device/{deviceId:int}/sensor/{sensorName}")]
        public async Task<ActionResult<Empty>> UploadSensorDataFromCsv([FromBody] SensorDataCsvResource csvResource,
            int deviceId, string sensorName, [FromQuery] bool allowOverwrite, CancellationToken token)
        {
            if (csvResource is null || string.IsNullOrWhiteSpace(csvResource.CsvContent))
                return BadRequest();

            var client = _dataService.GetClient(UserId);
            var call = client.SaveReadingsFromCsv(cancellationToken: token);
            var chunks = csvResource.CsvContent
                .Chunk(ChunkSize)
                .Select(ch => Encoding.UTF8.GetBytes(ch.ToArray()));

            try
            {
                foreach (var chunk in chunks)
                {
                    await call.RequestStream.WriteAsync(
                        new SaveReadingsFromCsvChunk
                        {
                            Chunk = ByteString.CopyFrom(chunk), AllowOverwrite = allowOverwrite,
                            DeviceId = deviceId,
                            SensorName = sensorName
                        });
                }

                await call.RequestStream.CompleteAsync();

                var result = await call;
                return Ok(result);
            }
            catch (RpcException e)
            {
                return HandleRpcException(e);
            }
        }

        [HttpGet("download/csv/sensor/{sensorId:int}/range/{startDate:datetime}/{endDate:datetime}")]
        public async Task<EmptyResult> GetRangeFromSensorAsCsv(int sensorId, DateTime startDate, DateTime endDate,
            CancellationToken token)
        {
            var request = new GetRangeFromSensorAsFileRequest
            {
                EndDate = Timestamp.FromDateTime(endDate.ToUniversalTime()),
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                FileType = FileType.Csv,
                SensorId = sensorId
            };

            await SendCsvFile(request, token);

            return new EmptyResult();
        }


        [HttpGet(
            "download/csv/device/{deviceId:int}/sensor/{sensorName}/range/{startDate:datetime}/{endDate:datetime}")]
        public async Task<EmptyResult> GetRangeFromSensorAsCsv(int deviceId, string sensorName, DateTime startDate,
            DateTime endDate,
            CancellationToken token)
        {
            var request = new GetRangeFromSensorAsFileRequest
            {
                EndDate = Timestamp.FromDateTime(endDate.ToUniversalTime()),
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                FileType = FileType.Csv,
                DeviceAndName = new DeviceAndSensorName { DeviceId = deviceId, SensorName = sensorName }
            };

            await SendCsvFile(request, token);

            return new EmptyResult();
        }

        private async Task SendCsvFile(GetRangeFromSensorAsFileRequest request, CancellationToken token)
        {
            Response.ContentType = "text/csv";
            var client = _dataService.GetClient(UserId);

            var call = client.GetRangeFromSensorAsFile(request, cancellationToken: token);

            await using var sw = new StreamWriter(Response.Body);
            await foreach (var chunk in call.ResponseStream.ReadAllAsync(token))
            {
                await sw.WriteAsync(Encoding.UTF8.GetString(chunk.FileContent.Span));
                await sw.FlushAsync();
            }
        }
    }
}