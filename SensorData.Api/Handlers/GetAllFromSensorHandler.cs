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
    public class GetAllFromSensorHandler : IRequestHandler<GetAllFromSensorQuery, GetManyFromSensorResponse>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllFromSensorHandler(IReadingService readingService, IMapper mapper,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IOptions<UserSettings> userSettings,
            IAppCache cache)
        {
            _readingService = readingService;
            _mapper = mapper;
            _sensorService = sensorService;
            _cache = cache;
            _userSettings = userSettings.Value;
        }

        public async Task<GetManyFromSensorResponse> Handle(GetAllFromSensorQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;
            SensorDto sensorDto;
            string cacheKey;

            // find sensor by name
            if (query.SensorCase == GetAllFromSensorRequest.SensorOneofCase.DeviceAndName)
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

            cacheKey =
                $"{nameof(_readingService.GetAllReadingsFromSensorQuery)}{sensorDto.Id}-{query.PageNumber}-{query.PageSize}";
            var pagedList = await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var readingsQuery = _readingService.GetAllReadingsFromSensorQuery(sensorDto.Id);
                return await PagedList<Reading>.ToPagedListAsync(readingsQuery, query.PageNumber,
                    query.PageSize, cancellationToken);
            }, TimeSpan.FromSeconds(15));

            var pagedListMapped = pagedList
                .Select(r => _mapper.Map<Reading, ReadingNoSensorDto>(r));

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