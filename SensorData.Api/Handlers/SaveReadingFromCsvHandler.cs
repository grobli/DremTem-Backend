using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Options;
using SensorData.Api.Commands;
using SensorData.Core.Models;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using Shared.Proto.Common;
using Shared.Proto.Device;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public class SaveReadingFromCsvHandler : IRequestHandler<SaveReadingsFromCsvCommand, Empty>
    {
        private readonly IReadingService _readingService;
        private readonly IGrpcService<DeviceGrpc.DeviceGrpcClient> _deviceService;
        private readonly UserSettings _userSettings;

        public SaveReadingFromCsvHandler(IReadingService readingService,
            IGrpcService<DeviceGrpc.DeviceGrpcClient> deviceService, IOptions<UserSettings> userSettings)
        {
            _readingService = readingService;
            _deviceService = deviceService;
            _userSettings = userSettings.Value;
        }

        public async Task<Empty> Handle(SaveReadingsFromCsvCommand command, CancellationToken cancellationToken)
        {
            int? deviceId = null;
            bool? allowOverwrite = null;
            await using var memoryStream = new MemoryStream();
            using var streamReader = new StreamReader(memoryStream);
            await foreach (var chunk in command.Stream.ReadAllAsync(cancellationToken))
            {
                deviceId ??= chunk.DeviceId;
                allowOverwrite ??= chunk.AllowOverwrite;
                await memoryStream.WriteAsync(chunk.Chunk.Memory, cancellationToken);
            }

            memoryStream.Position = 0;
            allowOverwrite ??= false;
            if (deviceId is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Missing device id"));
            }

            var deviceRequest = new GenericGetRequest
            {
                Id = deviceId.Value,
                Parameters = new GetRequestParameters
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

            var sensorIdDict = device.Sensors.ToDictionary(s => s.Name, s => s.Id);

            try
            {
                using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<CsvReading>()
                    .Select(r => new Reading
                    {
                        Time = DateTime.Parse(r.Time).ToUniversalTime(), Value = r.Value,
                        SensorId = sensorIdDict[r.SensorName]
                    }).ToList();
                Console.WriteLine(records.First());
                await _readingService.SaveManyReadingsAsync(records, allowOverwrite.Value, cancellationToken);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e));
            }

            return new Empty();
        }

        private record CsvReading(string Time, double Value, string SensorName);
    }
}