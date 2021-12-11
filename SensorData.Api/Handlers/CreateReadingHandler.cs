using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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

        public CreateReadingHandler(IReadingService readingService, IOptions<UserSettings> userSettings, IMapper mapper,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService)
        {
            _readingService = readingService;
            _userSettings = userSettings.Value;
            _mapper = mapper;
            _sensorService = sensorService;
        }

        public async Task<Empty> Handle(CreateReadingCommand command, CancellationToken cancellationToken)
        {
            // find sensor by name
            if (command.Body.SourceCase == CreateReadingRequest.SourceOneofCase.DeviceAndName)
            {
                var sensorRequest = new GetSensorByNameRequest
                {
                    DeviceId = command.Body.DeviceAndName.DeviceId,
                    SensorName = command.Body.DeviceAndName.SensorName,
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