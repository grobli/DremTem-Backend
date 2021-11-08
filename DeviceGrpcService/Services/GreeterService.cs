using System.Linq;
using System.Threading.Tasks;
using DeviceGrpcService.Data;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DeviceGrpcService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly DeviceContext _context;

        public GreeterService(ILogger<GreeterService> logger, DeviceContext context)
        {
            _logger = logger;
            _context = context;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name + _context.Devices.First()
            });
        }
    }
}