using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using SensorData.Api.Commands;
using SensorData.Core.Models;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using Shared.Proto.Common;
using Shared.Proto.Device;
using Shared.Proto.SensorData;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public class CreateReadingHandler : IRequestHandler<CreateReadingCommand, Empty>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<DeviceGrpc.DeviceGrpcClient> _deviceService;
        private readonly UserSettings _userSettings;
        private readonly IMapper _mapper;

        public CreateReadingHandler(IReadingService readingService,
            IGrpcService<DeviceGrpc.DeviceGrpcClient> deviceService, UserSettings userSettings, IMapper mapper)
        {
            _readingService = readingService;
            _deviceService = deviceService;
            _userSettings = userSettings;
            _mapper = mapper;
        }

        public async Task<Empty> Handle(CreateReadingCommand command, CancellationToken cancellationToken)
        {
            // find sensor id
            if (command.Body.SourceCase == CreateReadingRequest.SourceOneofCase.DeviceAndName)
            {
                var deviceRequest = new GenericGetRequest
                {
                    Id = command.Body.DeviceAndName.DeviceId, Parameters = new GetRequestParameters
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

                command.Body.SensorId =
                    device.Sensors.FirstOrDefault(s => s.Name == command.Body.DeviceAndName.SensorName)?.Id ?? -1;
                if (command.Body.SensorId == -1)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Sensor not found"));
                }
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