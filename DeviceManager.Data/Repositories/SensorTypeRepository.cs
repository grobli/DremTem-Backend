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
    public class SensorTypeRepository : ISensorTypeRepository
    {
        private readonly IDeviceManagerContext _context;

        public SensorTypeRepository(IDeviceManagerContext context)
        {
            _context = context;
        }

        public IQueryable<SensorType> GetSensorTypeById(int typeId)
        {
            return _context.SensorTypes
                .Where(st => st.Id == typeId)
                .Take(1);
        }


        public async Task<SensorType> AddAsync(SensorType entity, CancellationToken cancellationToken = default)
        {
            var result = await _context.SensorTypes.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<SensorType> entities, CancellationToken cancellationToken = default)
        {
            await _context.SensorTypes.AddRangeAsync(entities, cancellationToken);
        }

        public IQueryable<SensorType> FindAll()
        {
            return _context.SensorTypes;
        }

        public async Task<IEnumerable<SensorType>> FindAsync(Expression<Func<SensorType, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.SensorTypes.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<SensorType> SingleOrDefaultAsync(Expression<Func<SensorType, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.SensorTypes.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(SensorType entity)
        {
            _context.SensorTypes.Remove(entity);
        }

        public void RemoveRange(IEnumerable<SensorType> entities)
        {
            _context.SensorTypes.RemoveRange(entities);
        }
    }
}