using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Shared.Proto;

namespace SensorData.Api.Queries
{
    public class GetRangeFromSensorAsFileQuery : IRequest<Empty>
    {
        public GetRangeFromSensorAsFileQuery(GetRangeFromSensorAsFileRequest query,
            IServerStreamWriter<GetRangeFromSensorFileChunk> stream)
        {
            Stream = stream;
            Query = query;
        }

        public IServerStreamWriter<GetRangeFromSensorFileChunk> Stream { get; }
        public GetRangeFromSensorAsFileRequest Query { get; }
    }
}