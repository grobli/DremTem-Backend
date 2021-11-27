using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Repositories;
using DeviceManager.Data.Repositories;

namespace DeviceManager.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DeviceManagerContext _context;
        private DeviceRepository _deviceRepository;
        private LocationRepository _locationRepository;
        private SensorRepository _sensorRepository;
        private SensorTypeRepository _typeRepository;

        public IDeviceRepository Devices => _deviceRepository ??= new DeviceRepository(_context);
        public ILocationRepository Locations => _locationRepository ??= new LocationRepository(_context);
        public ISensorRepository Sensors => _sensorRepository ??= new SensorRepository(_context);
        public ISensorTypeRepository SensorTypes => _typeRepository ??= new SensorTypeRepository(_context);

        public UnitOfWork(DeviceManagerContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}