using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SensorData.Core;
using SensorData.Core.Models;
using SensorData.Core.Repositories;

namespace SensorData.Data.Repositories
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly ISensorDataContext _context;

        public ReadingRepository(ISensorDataContext context)
        {
            _context = context;
        }

        public IQueryable<Reading> GetReadings()
        {
            return _context.Readings.OrderBy(r => r.Time);
        }

        public IQueryable<Reading> GetReadingsBySensorId(int sensorId)
        {
            return _context.Readings.Where(r => r.SensorId == sensorId).OrderBy(r => r.Time);
        }

        public async Task<Reading> GetReadingByTimestampAsync(DateTime timestamp, int sensorId, CancellationToken token)
        {
            return await SingleOrDefaultAsync(r => r.Time == timestamp && r.SensorId == sensorId, token);
        }


        public async Task<Reading> AddAsync(Reading entity, CancellationToken cancellationToken = default)
        {
            var result = await _context.Readings.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<Reading> entities, CancellationToken cancellationToken = default)
        {
            await _context.Readings.AddRangeAsync(entities, cancellationToken);
        }

        public IQueryable<Reading> FindAll()
        {
            return _context.Readings;
        }

        public async Task<IEnumerable<Reading>> FindAsync(Expression<Func<Reading, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Readings.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<Reading> SingleOrDefaultAsync(Expression<Func<Reading, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Readings.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(Reading entity)
        {
            _context.Readings.Remove(entity);
        }

        public void RemoveRange(IEnumerable<Reading> entities)
        {
            _context.Readings.RemoveRange(entities);
        }
    }
}