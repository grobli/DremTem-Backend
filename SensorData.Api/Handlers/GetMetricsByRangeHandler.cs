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

        public GetMetricsByRangeHandler(IMetricService metricService,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IOptions<UserSettings> userSettings,
            IMapper mapper)
        {
            _metricService = metricService;
            _sensorService = sensorService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
        }

        public async Task<GetMetricsByRangeResponse> Handle(GetDailyMetricsByRangeQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;
            SensorDto sensorDto;

            // find sensor by name
            if (query.SensorCase == GetMetricsByRangeRequest.SensorOneofCase.DeviceAndName)
            {
                var sensorRequest = new GetSensorByNameRequest
                {
                    DeviceId = query.DeviceAndName.DeviceId,
                    SensorName = query.DeviceAndName.SensorName,
                    Parameters = new GetRequestParameters { UserId = _userSettings.Id.ToString() }
                };
                sensorDto = await _sensorService.SendRequestAsync(async client =>
                    await client.GetSensorByNameAsync(sensorRequest));
            }
            else
            {
                var sensorRequest = new GenericGetRequest
                {
                    Id = query.SensorId, Parameters = new GetRequestParameters { UserId = _userSettings.Id.ToString() }
                };
                sensorDto = await _sensorService.SendRequestAsync(async client =>
                    await client.GetSensorAsync(sensorRequest));
            }

            if (sensorDto is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Sensor not found"));
            }

            var paginationParams = new PaginationParameters { PageNumber = query.PageNumber, PageSize = query.PageSize };
            var metrics = await _metricService.GetMetricsByRange(sensorDto.Id, request.MetricMode, paginationParams,
                query.StartDate.ToDateTime(), query.EndDate.ToDateTime());

            var metricsMapped = metrics.Select(m => _mapper.Map<MetricBase, MetricDto>(m));

            var response = new GetMetricsByRangeResponse
            {
                SensorId = sensorDto.Id,
                SensorTypeId = sensorDto.TypeId,
                PaginationMetaData = new PaginationMetaData().FromPagedList(metrics),
                Metrics = { metricsMapped }
            };

            return response;
        }
    }
}