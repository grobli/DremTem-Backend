using System;
using System.Threading;
using System.Threading.Tasks;
using SensorData.Core.Repositories;

namespace SensorData.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IReadingRepository Readings { get; }
        Task<int> CommitAsync(CancellationToken token = default);
    }
}