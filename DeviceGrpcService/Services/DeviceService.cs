using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceGrpcService.Data;
using DeviceGrpcService.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Device = DeviceGrpcService.Proto.Device;

namespace DeviceGrpcService.Services
{
    public class DeviceService : Device.DeviceBase
    {
        private readonly ILogger<DeviceService> _logger;
        private readonly DeviceContext _context;
        private readonly IMapper _mapper;

        public DeviceService(ILogger<DeviceService> logger, DeviceContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public override async Task GetAllDevices(Empty request, IServerStreamWriter<DeviceGrpcBaseModel> responseStream,
            ServerCallContext context)
        {
            foreach (var device in _context.Devices)
            {
                await responseStream.WriteAsync(_mapper.Map<DeviceGrpcBaseModel>(device));
            }
        }

        public override async Task GetAllDevicesNested(Empty request,
            IServerStreamWriter<DeviceGrpcNestedModel> responseStream, ServerCallContext context)
        {
            foreach (var device in _context.Devices
                .Include(d => d.Location))
            {
                await responseStream.WriteAsync(_mapper.Map<DeviceGrpcNestedModel>(device));
            }
        }
    }
}