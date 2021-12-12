using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Options;
using SensorData.Api.Commands;
using SensorData.Core.Models;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using Shared.Proto;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public class CreateReadingHandler : IRequestHandler<CreateReadingCommand, Empty>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public CreateReadingHandler(IReadingService readingService, IOptions<UserSettings> userSettings, IMapper mapper,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService, IAppCache cache)
        {
            _readingService = readingService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
            _sensorService = sensorService;
            _cache = cache;
        }

        public async Task<Empty> Handle(CreateReadingCommand command, CancellationToken cancellationToken)
        {
            // find sensor by name
            if (command.Body.SourceCase == CreateReadingRequest.SourceOneofCase.DeviceAndName)
            {
                var deviceId = command.Body.DeviceAndName.DeviceId;
                var sensorName = command.Body.DeviceAndName.SensorName;
                var cacheKey = $"{nameof(HandlerUtils.FindSensorByName)}{deviceId}{sensorName}";

                var sensorDto = await _cache.GetOrAddAsync(cacheKey,
                    async () => await HandlerUtils.FindSensorByName(_sensorService, deviceId, sensorName,
                        _userSettings.Id), TimeSpan.FromMinutes(15));

                if (sensorDto is null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Sensor not found"));
                }

                command.Body.SensorId = sensorDto.Id;
            }

            var reading = _mapper.Map<CreateReadingRequest, Reading>(command.Body);
            try
            {
                await _readingService.SaveReadingAsync(reading, command.Body.AllowOverwrite, cancellationToken);
                return new Empty();
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }
        }
    }
}