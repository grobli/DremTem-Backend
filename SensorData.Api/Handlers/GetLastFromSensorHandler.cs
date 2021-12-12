using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Options;
using SensorData.Api.Queries;
using SensorData.Core.Models;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using Shared;
using Shared.Extensions;
using Shared.Proto;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public class GetLastFromSensorHandler : IRequestHandler<GetLastFromSensorQuery, GetManyFromSensorResponse>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetLastFromSensorHandler(IReadingService readingService,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IOptions<UserSettings> userSettings,
            IMapper mapper, IAppCache cache)
        {
            _readingService = readingService;
            _sensorService = sensorService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<GetManyFromSensorResponse> Handle(GetLastFromSensorQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;
            SensorDto sensorDto;
            string cacheKey;

            // find sensor by name
            if (query.SensorCase == GetLastFromSensorRequest.SensorOneofCase.DeviceAndName)
            {
                var deviceId = query.DeviceAndName.DeviceId;
                var sensorName = query.DeviceAndName.SensorName;
                cacheKey = $"{nameof(HandlerUtils.FindSensorByName)}{deviceId}{sensorName}";
                sensorDto = await _cache.GetOrAddAsync(cacheKey,
                    async () => await HandlerUtils.FindSensorByName(_sensorService, deviceId, sensorName,
                        _userSettings.Id), TimeSpan.FromMinutes(15));
            }
            else
            {
                var sensorRequest = new GenericGetRequest
                {
                    Id = query.SensorId,
                    Parameters = new GetRequestParameters { UserId = _userSettings.Id.ToString() }
                };
                cacheKey = $"{nameof(SensorDto)}{query.SensorId}";
                sensorDto = await _cache.GetOrAddAsync(cacheKey,
                    async () => await _sensorService.SendRequestAsync(async client =>
                        await client.GetSensorAsync(sensorRequest)), TimeSpan.FromMinutes(15));
            }
            
            if (sensorDto is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Sensor not found"));
            }

            var dateOffset = query.TimeUnit switch
            {
                TimeUnit.Second => DateTime.UtcNow - TimeSpan.FromSeconds(query.TimeUnitValue),
                TimeUnit.Minute => DateTime.UtcNow - TimeSpan.FromMinutes(query.TimeUnitValue),
                TimeUnit.Hour => DateTime.UtcNow - TimeSpan.FromHours(query.TimeUnitValue),
                TimeUnit.Day => DateTime.UtcNow - TimeSpan.FromDays(query.TimeUnitValue),
                _ => throw new ArgumentOutOfRangeException(nameof(request))
            };

            cacheKey = $"{GetType().Name}{sensorDto.Id}-{query.TimeUnit}-{query.TimeUnitValue}";
            var pagedList = await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var readingsQuery = _readingService.GetAllReadingsFromSensorQuery(sensorDto.Id)
                    .Where(r => r.Time > dateOffset);
                return await PagedList<Reading>.ToPagedListAsync(readingsQuery, query.PageNumber, query.PageSize,
                    cancellationToken);
            }, TimeSpan.FromSeconds(15));
            var pagedListMapped = pagedList.Select(r => _mapper.Map<Reading, ReadingNoSensorDto>(r));

            var response = new GetManyFromSensorResponse
            {
                Readings = { pagedListMapped },
                SensorId = sensorDto.Id,
                SensorTypeId = sensorDto.TypeId,
                PaginationMetaData = new PaginationMetaData().FromPagedList(pagedList)
            };

            return response;
        }
    }
}