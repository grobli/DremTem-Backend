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
using Shared.Proto;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public class SaveReadingFromCsvHandler : IRequestHandler<SaveReadingsFromCsvCommand, Empty>
    {
        private readonly IReadingService _readingService;
        private readonly UserSettings _userSettings;
        private readonly IGrpcService<SensorGrpc.SensorGrpcClient> _sensorService;

        public SaveReadingFromCsvHandler(IReadingService readingService, IOptions<UserSettings> userSettings,
            IGrpcService<SensorGrpc.SensorGrpcClient> sensorService)
        {
            _readingService = readingService;
            _sensorService = sensorService;
            _userSettings = userSettings.Value;
        }

        public async Task<Empty> Handle(SaveReadingsFromCsvCommand command, CancellationToken cancellationToken)
        {
            int? deviceId = null;
            bool? allowOverwrite = null;
            string sensorName = null;
            await using var memoryStream = new MemoryStream();
            using var streamReader = new StreamReader(memoryStream);
            await foreach (var chunk in command.Stream.ReadAllAsync(cancellationToken))
            {
                deviceId ??= chunk.DeviceId;
                allowOverwrite ??= chunk.AllowOverwrite;
                sensorName ??= chunk.SensorName;
                await memoryStream.WriteAsync(chunk.Chunk.Memory, cancellationToken);
            }

            memoryStream.Position = 0;
            allowOverwrite ??= false;
            if (deviceId is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Missing device id"));
            }

            if (sensorName is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Missing sensor name"));
            }

            var sensorRequest = new GetSensorByNameRequest
            {
                DeviceId = deviceId.Value, SensorName = sensorName,
                Parameters = new GetRequestParameters { UserId = _userSettings.Id.ToString() }
            };
            var sensor = await _sensorService.SendRequestAsync(async client =>
                await client.GetSensorByNameAsync(sensorRequest));
            if (sensor is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Sensor not found"));
            }

            try
            {
                using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<CsvReading>()
                    .Select(r => new Reading
                    {
                        Time = DateTime.Parse(r.Time).ToUniversalTime(), Value = r.Reading,
                        SensorId = sensor.Id
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

        private record CsvReading(string Time, double Reading);
    }
}