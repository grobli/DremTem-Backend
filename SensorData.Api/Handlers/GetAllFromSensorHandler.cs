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
using Shared.Proto.Common;
using Shared.Proto.Sensor;
using Shared.Proto.SensorData;
using Shared.Services.GrpcClientServices;


namespace SensorData.Api.Handlers
{
    public class GetAllFromSensorHandler : IRequestHandler<GetAllFromSensorQuery, GetManyFromSensorResponse>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;

        public GetAllFromSensorHandler(IReadingService readingService, IMapper mapper,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IOptions<UserSettings> userSettings)
        {
            _readingService = readingService;
            _mapper = mapper;
            _sensorService = sensorService;
            _userSettings = userSettings.Value;
        }

        public async Task<GetManyFromSensorResponse> Handle(GetAllFromSensorQuery request,
            CancellationToken cancellationToken)
        {
            var query = request.Query;
            SensorDto sensorDto;

            // find sensor by name
            if (query.SensorCase == GetAllFromSensorRequest.SensorOneofCase.DeviceAndName)
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

            var readingsQuery = _readingService.GetAllReadingsFromSensorQuery(sensorDto.Id);
            var pagedList = await PagedList<Reading>.ToPagedListAsync(readingsQuery, query.PageNumber, query.PageSize,
                cancellationToken);
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