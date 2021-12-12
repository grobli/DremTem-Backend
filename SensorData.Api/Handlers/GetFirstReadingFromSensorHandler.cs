using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SensorData.Api.Queries;
using SensorData.Core.Models;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using Shared.Proto;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public class GetFirstReadingFromSensorHandler : IRequestHandler<GetFirstReadingFromSensorQuery, ReadingDto>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetFirstReadingFromSensorHandler(IReadingService readingService,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IOptions<UserSettings> userSettings,
            IMapper mapper, IAppCache cache)
        {
            _readingService = readingService;
            _sensorService = sensorService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ReadingDto> Handle(GetFirstReadingFromSensorQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;

            // find sensor by name
            if (query.SensorCase == GetFirstRecentFromSensorRequest.SensorOneofCase.DeviceAndName)
            {
                var deviceId = query.DeviceAndName.DeviceId;
                var sensorName = query.DeviceAndName.SensorName;
                var cacheKey = $"{nameof(HandlerUtils.FindSensorByName)}{deviceId}{sensorName}";

                var sensorDto = await _cache.GetOrAddAsync(cacheKey,
                    async () => await HandlerUtils.FindSensorByName(_sensorService, deviceId, sensorName,
                        _userSettings.Id), TimeSpan.FromMinutes(15));

                if (sensorDto is null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Sensor not found"));
                }

                query.SensorId = sensorDto.Id;
            }

            var reading = await _readingService.GetAllReadingsFromSensorQuery(query.SensorId)
                .FirstAsync(cancellationToken);

            return _mapper.Map<Reading, ReadingDto>(reading);
        }
    }
}