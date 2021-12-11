using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
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

        public GetFirstReadingFromSensorHandler(IReadingService readingService,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IOptions<UserSettings> userSettings,
            IMapper mapper)
        {
            _readingService = readingService;
            _sensorService = sensorService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
        }

        public async Task<ReadingDto> Handle(GetFirstReadingFromSensorQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;

            // find sensor by name
            if (query.SensorCase == GetFirstRecentFromSensorRequest.SensorOneofCase.DeviceAndName)
            {
                var sensorRequest = new GetSensorByNameRequest
                {
                    DeviceId = query.DeviceAndName.DeviceId,
                    SensorName = query.DeviceAndName.SensorName,
                    Parameters = new GetRequestParameters
                    {
                        UserId = _userSettings.Id.ToString()
                    }
                };
                var sensorDto = await _sensorService.SendRequestAsync(async client =>
                    await client.GetSensorByNameAsync(sensorRequest));
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