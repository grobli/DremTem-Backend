using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SensorData.Core.Models;

namespace SensorData.Core.Services
{
    public interface IReadingService
    {
        IQueryable<Reading> GetAllReadingsQuery();
        IQueryable<Reading> GetAllReadingsFromSensorQuery(int sensorId);
        Task<Reading> GetReadingByTimestampAsync(DateTime timestamp, int sensorId, CancellationToken token = default);

        Task<Reading> SaveReadingAsync(Reading reading, bool allowOverwrite = false, CancellationToken token = default);

        Task SaveManyReadingsAsync(IEnumerable<Reading> readings, bool allowOverwrite = false,
            CancellationToken token = default);

        Task UpdateReadingAsync(Reading readingToUpdate, double value, CancellationToken token = default);

        Task DeleteReadingAsync(Reading reading, CancellationToken token = default);
        Task DeleteReadingsRangeAsync(IEnumerable<Reading> readings, CancellationToken token = default);
    }
}