using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SensorData.Core.Models;
using Shared.Repositories;

namespace SensorData.Core.Repositories
{
    public interface IReadingRepository : IRepository<Reading>
    {
        IQueryable<Reading> GetReadings();
        IQueryable<Reading> GetReadingsBySensorId(int sensorId);
        Task<Reading> GetReadingByTimestampAsync(DateTime timestamp, int sensorId, CancellationToken token = default);
    }
}