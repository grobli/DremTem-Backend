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
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IDeviceManagerContext _context;

        public DeviceRepository(IDeviceManagerContext context)
        {
            _context = context;
        }

        public IQueryable<Device> GetDevices(Guid userId)
        {
            var devices = userId == Guid.Empty
                ? _context.Devices
                : _context.Devices.Where(d => d.UserId == userId);

            return devices.OrderBy(d => d.Name);
        }

        public IQueryable<Device> GetDeviceById(int deviceId, Guid userId = default)
        {
            var devices = userId == Guid.Empty
                ? _context.Devices
                : _context.Devices.Where(d => d.UserId == userId);

            return devices.Where(d => d.Id == deviceId).Take(1);
        }

        public async Task<Device> AddAsync(Device entity, CancellationToken cancellationToken = default)
        {
            var result = await _context.Devices.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<Device> entities, CancellationToken cancellationToken = default)
        {
            await _context.Devices.AddRangeAsync(entities, cancellationToken);
        }

        public IQueryable<Device> FindAll()
        {
            return _context.Devices;
        }

        public async Task<IEnumerable<Device>> FindAsync(Expression<Func<Device, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Devices.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<Device> SingleOrDefaultAsync(Expression<Func<Device, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Devices.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(Device entity)
        {
            _context.Devices.Remove(entity);
        }

        public void RemoveRange(IEnumerable<Device> entities)
        {
            _context.Devices.RemoveRange(entities);
        }
    }
}