using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Shared.Proto.SensorData;

namespace SensorData.Api.Commands
{
    public class SaveReadingsFromCsvCommand : IRequest<Empty>
    {
        public SaveReadingsFromCsvCommand(IAsyncStreamReader<SaveReadingsFromCsvChunk> stream)
        {
            Stream = stream;
        }

        public IAsyncStreamReader<SaveReadingsFromCsvChunk> Stream { get; }
    }
}