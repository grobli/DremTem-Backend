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
    public class LocationRepository : ILocationRepository
    {
        private readonly IDeviceManagerContext _context;

        public LocationRepository(IDeviceManagerContext context)
        {
            _context = context;
        }

        public IQueryable<Location> GetLocations(Guid userId)
        {
            var locations = userId == Guid.Empty
                ? _context.Locations
                : _context.Locations.Where(l => l.UserId == userId);

            return locations.OrderBy(l => l.Name);
        }

        public IQueryable<Location> GetLocationById(int locationId, Guid userId)
        {
            var locations = userId == Guid.Empty
                ? _context.Locations
                : _context.Locations.Where(d => d.UserId == userId);

            return locations.Where(l => l.Id == locationId).Take(1);
        }

        public async Task<Location> AddAsync(Location entity, CancellationToken cancellationToken = default)
        {
            var result = await _context.Locations.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<Location> entities, CancellationToken cancellationToken = default)
        {
            await _context.Locations.AddRangeAsync(entities, cancellationToken);
        }

        public IQueryable<Location> FindAll()
        {
            return _context.Locations;
        }

        public async Task<IEnumerable<Location>> FindAsync(Expression<Func<Location, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Locations.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<Location> SingleOrDefaultAsync(Expression<Func<Location, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Locations.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(Location entity)
        {
            _context.Locations.Remove(entity);
        }

        public void RemoveRange(IEnumerable<Location> entities)
        {
            _context.Locations.RemoveRange(entities);
        }
    }
}