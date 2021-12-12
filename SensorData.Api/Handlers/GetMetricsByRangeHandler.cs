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
    public class GetMetricsByRangeHandler : IRequestHandler<GetDailyMetricsByRangeQuery, GetMetricsByRangeResponse>
    {
        private readonly IMetricService _metricService;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetMetricsByRangeHandler(IMetricService metricService,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IOptions<UserSettings> userSettings,
            IMapper mapper, IAppCache cache)
        {
            _metricService = metricService;
            _sensorService = sensorService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<GetMetricsByRangeResponse> Handle(GetDailyMetricsByRangeQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;
            SensorDto sensorDto;
            string cacheKey;

            // find sensor by name
            if (query.SensorCase == GetMetricsByRangeRequest.SensorOneofCase.DeviceAndName)
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

            var paginationParams = new PaginationParameters
                { PageNumber = query.PageNumber, PageSize = query.PageSize };
            cacheKey =
                $"{GetType().Name}{sensorDto.Id}-{request.MetricMode}-{query.StartDate}-{query.EndDate}-{paginationParams}";

            var pagedList = await _cache.GetOrAddAsync(cacheKey, async () => await _metricService.GetMetricsByRange(
                sensorDto.Id, request.MetricMode, paginationParams,
                query.StartDate.ToDateTime(), query.EndDate.ToDateTime()), TimeSpan.FromSeconds(15));

            var metricsMapped = pagedList.Select(m => _mapper.Map<MetricBase, MetricDto>(m));

            var response = new GetMetricsByRangeResponse
            {
                SensorId = sensorDto.Id,
                SensorTypeId = sensorDto.TypeId,
                PaginationMetaData = new PaginationMetaData().FromPagedList(pagedList),
                Metrics = { metricsMapped }
            };

            return response;
        }
    }
}