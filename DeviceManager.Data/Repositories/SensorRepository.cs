using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly IDeviceManagerContext _context;

        public SensorRepository(IDeviceManagerContext context)
        {
            _context = context;
        }

        public IQueryable<Sensor> GetSensors(Guid userId)
        {
            var sensors = userId == Guid.Empty
                ? _context.Sensors
                : _context.Sensors.Where(s => s.Device.UserId == userId);

            return sensors.OrderBy(d => d.Name);
        }

        public IQueryable<Sensor> GetSensorById(int sensorId, Guid userId)
        {
            var sensors = userId == Guid.Empty
                ? _context.Sensors
                : _context.Sensors.Where(s => s.Device.UserId == userId);

            return sensors.Where(d => d.Id == sensorId).Take(1);
        }

        public async Task<Sensor> AddAsync(Sensor entity, CancellationToken cancellationToken = default)
        {
            var result = await _context.Sensors.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<Sensor> entities, CancellationToken cancellationToken = default)
        {
            await _context.Sensors.AddRangeAsync(entities, cancellationToken);
        }

        public IQueryable<Sensor> FindAll()
        {
            return _context.Sensors;
        }

        public async Task<IEnumerable<Sensor>> FindAsync(Expression<Func<Sensor, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Sensors.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<Sensor> SingleOrDefaultAsync(Expression<Func<Sensor, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Sensors.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(Sensor entity)
        {
            _context.Sensors.Remove(entity);
        }

        public void RemoveRange(IEnumerable<Sensor> entities)
        {
            _context.Sensors.RemoveRange(entities);
        }
    }
}