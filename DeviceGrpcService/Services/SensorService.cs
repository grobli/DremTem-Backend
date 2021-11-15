using AutoMapper;
using DeviceGrpcService.Data;
using DeviceGrpcService.Proto;
using Microsoft.Extensions.Logging;

namespace DeviceGrpcService.Services
{
    public class SensorService : Sensor.SensorBase
    {
        private readonly ILogger<SensorService> _logger;
        private readonly DeviceContext _context;
        private readonly IMapper _mapper;

        public SensorService(ILogger<SensorService> logger, DeviceContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
    }
}