using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SensorData.Core.Proto;

namespace SensorData.Api.Services
{
    public class SensorDataService : SensorDataGrpc.SensorDataGrpcBase
    {
        private readonly ILogger<SensorDataService> _logger;

        public SensorDataService(ILogger<SensorDataService> logger)
        {
            _logger = logger;
        }

        public override Task GetRangeFromSensorAsFile(GetRangeFromSensorAsFileRequest request,
            IServerStreamWriter<GetRangeFromSensorFileChunk> responseStream,
            ServerCallContext context)
        {
            return base.GetRangeFromSensorAsFile(request, responseStream, context);
        }
    }
}