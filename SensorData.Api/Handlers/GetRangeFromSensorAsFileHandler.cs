using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using SensorData.Api.Queries;
using Shared.Proto.SensorData;

namespace SensorData.Api.Handlers
{
    public class GetRangeFromSensorAsFileHandler : IRequestHandler<GetRangeFromSensorAsFileQuery, Empty>
    {
        private const int PageSize = 2000;
        private readonly IMediator _mediator;

        public GetRangeFromSensorAsFileHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Empty> Handle(GetRangeFromSensorAsFileQuery request, CancellationToken cancellationToken)
        {
            var getRangeRequest = new GetRangeFromSensorRequest
            {
                EndDate = request.Query.EndDate,
                StartDate = request.Query.StartDate,
                PageSize = PageSize,
                PageNumber = 1
            };
            if (request.Query.SensorCase == GetRangeFromSensorAsFileRequest.SensorOneofCase.SensorId)
                getRangeRequest.SensorId = request.Query.SensorId;
            else getRangeRequest.DeviceAndName = request.Query.DeviceAndName;

            var result = await _mediator.Send(new GetRangeFromSensorQuery(getRangeRequest), cancellationToken);

            MemoryStream memoryStream;
            StreamWriter textWriter;
            CsvWriter csvWriter;

            await using (memoryStream = new MemoryStream())
            await using (textWriter = new StreamWriter(memoryStream))
            await using (csvWriter = new CsvWriter(textWriter, CultureInfo.InvariantCulture))
            {
                // write first part with header
                await csvWriter.WriteRecordsAsync(result.Readings
                    .Select(r => new CsvRecord(r.Time.ToDateTime().ToString("O"), r.Value)), cancellationToken);

                await csvWriter.FlushAsync();
                memoryStream.Position = 0;
                // send first part
                var chunk = new GetRangeFromSensorFileChunk
                {
                    ChunkNumber = 1, FileType = FileType.Csv,
                    FileContent = await ByteString.FromStreamAsync(memoryStream, cancellationToken)
                };
                await request.Stream.WriteAsync(chunk);
            }


            // send the rest
            while (result.PaginationMetaData.HasNext)
            {
                await using (memoryStream = new MemoryStream())
                await using (textWriter = new StreamWriter(memoryStream))
                await using (csvWriter = new CsvWriter(textWriter,
                    new CsvConfiguration(CultureInfo.InvariantCulture)
                        { HasHeaderRecord = false }))
                {
                    getRangeRequest.PageNumber += 1;
                    result = await _mediator.Send(new GetRangeFromSensorQuery(getRangeRequest), cancellationToken);

                    await csvWriter.WriteRecordsAsync(result.Readings
                        .Select(r => new CsvRecord(r.Time.ToDateTime().ToString("O"), r.Value)), cancellationToken);
                    await csvWriter.FlushAsync();

                    memoryStream.Position = 0;
                    var chunk = new GetRangeFromSensorFileChunk
                    {
                        ChunkNumber = getRangeRequest.PageNumber, FileType = FileType.Csv,
                        FileContent = await ByteString.FromStreamAsync(memoryStream, cancellationToken)
                    };

                    await request.Stream.WriteAsync(chunk);
                }
            }

            return new Empty();
        }

        private record CsvRecord(string Time, double Value);
    }
}