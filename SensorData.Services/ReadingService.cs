using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SensorData.Core;
using SensorData.Core.Models;
using SensorData.Core.Services;

namespace SensorData.Services
{
    public class ReadingService : IReadingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReadingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Reading> GetAllReadingsQuery()
        {
            return _unitOfWork.Readings.GetReadings();
        }

        public IQueryable<Reading> GetAllReadingsFromSensorQuery(int sensorId)
        {
            return _unitOfWork.Readings.GetReadingsBySensorId(sensorId);
        }

        public async Task<Reading> GetReadingByTimestampAsync(DateTime timestamp, int sensorId,
            CancellationToken token = default)
        {
            return await _unitOfWork.Readings.GetReadingByTimestampAsync(timestamp, sensorId, token);
        }

        public async Task<Reading> SaveReadingAsync(Reading reading, bool allowOverwrite = false,
            CancellationToken token = default)
        {
            await AddReadingAsync(reading, allowOverwrite, token);
            await _unitOfWork.CommitAsync(token);
            return reading;
        }


        public async Task SaveManyReadingsAsync(IEnumerable<Reading> readings, bool allowOverwrite = false,
            CancellationToken token = default)
        {
            foreach (var reading in readings)
            {
                await AddReadingAsync(reading, allowOverwrite, token);
            }

            await _unitOfWork.CommitAsync(token);
        }

        public async Task UpdateReadingAsync(Reading readingToUpdate, double value, CancellationToken token = default)
        {
            readingToUpdate.Value = value;
            await _unitOfWork.CommitAsync(token);
        }

        public async Task DeleteReadingAsync(Reading reading, CancellationToken token = default)
        {
            _unitOfWork.Readings.Remove(reading);
            await _unitOfWork.CommitAsync(token);
        }

        public async Task DeleteReadingsRangeAsync(IEnumerable<Reading> readings, CancellationToken token = default)
        {
            _unitOfWork.Readings.RemoveRange(readings);
            await _unitOfWork.CommitAsync(token);
        }

        private async Task AddReadingAsync(Reading reading, bool allowOverwrite = false,
            CancellationToken token = default)
        {
            // check if reading already exist
            var existing = await _unitOfWork.Readings.GetReadingByTimestampAsync(reading.Time, reading.SensorId, token);
            if (existing is null)
            {
                await _unitOfWork.Readings.AddAsync(reading, token);
            }
            else // reading already exists
            {
                if (!allowOverwrite)
                    throw new ArgumentException(
                        "Failed to save the reading. " +
                        "The reading with this timestamp and sensor id already exists. " +
                        "Set \"allowOverwrite\" arg to true to overwrite existing values",
                        nameof(reading));
                existing.Value = reading.Value;
            }
        }
    }
}