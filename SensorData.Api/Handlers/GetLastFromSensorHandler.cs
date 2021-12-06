using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Options;
using SensorData.Api.Queries;
using SensorData.Core.Models;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using Shared;
using Shared.Extensions;
using Shared.Proto.Common;
using Shared.Proto.Device;
using Shared.Proto.Sensor;
using Shared.Proto.SensorData;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public class GetLastFromSensorHandler : IRequestHandler<GetLastFromSensorQuery, GetManyFromSensorResponse>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;
        private readonly IGrpcService<DeviceGrpc.DeviceGrpcClient> _deviceService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;

        public GetLastFromSensorHandler(IReadingService readingService,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService,
            IGrpcService<DeviceGrpc.DeviceGrpcClient> deviceService, IOptions<UserSettings> userSettings, IMapper mapper)
        {
            _readingService = readingService;
            _sensorService = sensorService;
            _deviceService = deviceService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
        }

        public async Task<GetManyFromSensorResponse> Handle(GetLastFromSensorQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;

            // find sensor id
            if (query.SensorCase == GetLastFromSensorRequest.SensorOneofCase.DeviceAndName)
            {
                var deviceRequest = new GenericGetRequest
                {
                    Id = query.DeviceAndName.DeviceId, Parameters = new GetRequestParameters
                    {
                        UserId = _userSettings.Id.ToString(),
                        IncludeFields = { Entity.Sensor }
                    }
                };
                var device = await _deviceService.SendRequestAsync(async client =>
                    await client.GetDeviceAsync(deviceRequest));
                if (device is null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Device not found"));
                }

                query.SensorId = device.Sensors.FirstOrDefault(s => s.Name == query.DeviceAndName.SensorName)?.Id ?? -1;
                if (query.SensorId == -1)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Sensor not found"));
                }
            }

            var sensorRequest = new GenericGetRequest
                { Id = query.SensorId, Parameters = new GetRequestParameters { UserId = _userSettings.Id.ToString() } };
            var sensor = await _sensorService.SendRequestAsync(async client =>
                await client.GetSensorAsync(sensorRequest));
            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    "Sensor not found")); // maybe it was deleted in the meantime who knows?
            }

            var dateOffset = query.TimeUnit switch
            {
                TimeUnit.Second => DateTime.UtcNow - TimeSpan.FromSeconds(query.TimeUnitValue),
                TimeUnit.Minute => DateTime.UtcNow - TimeSpan.FromMinutes(query.TimeUnitValue),
                TimeUnit.Hour => DateTime.UtcNow - TimeSpan.FromHours(query.TimeUnitValue),
                TimeUnit.Day => DateTime.UtcNow - TimeSpan.FromDays(query.TimeUnitValue),
                _ => throw new ArgumentOutOfRangeException(nameof(request))
            };

            var readingsQuery = _readingService.GetAllReadingsFromSensorQuery(sensor.Id)
                .Where(r => r.Time > dateOffset);
            var pagedList = await PagedList<Reading>.ToPagedListAsync(readingsQuery, query.PageNumber, query.PageSize,
                cancellationToken);
            var pagedListMapped = pagedList.Select(r => _mapper.Map<Reading, ReadingNoSensorDto>(r));

            var response = new GetManyFromSensorResponse
            {
                Readings = { pagedListMapped },
                SensorId = sensor.Id,
                SensorTypeId = sensor.TypeId,
                PaginationMetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;
        }
    }
}