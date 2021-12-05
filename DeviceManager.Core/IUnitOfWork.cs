using System;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Repositories;

namespace DeviceManager.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IDeviceRepository Devices { get; }
        ILocationRepository Locations { get; }
        ISensorRepository Sensors { get; }
        ISensorTypeRepository SensorTypes { get; }
        IGroupRepository Groups { get; }

        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}