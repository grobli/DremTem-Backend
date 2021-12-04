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
    public class GroupRepository : IGroupRepository
    {
        private readonly IDeviceManagerContext _context;

        public GroupRepository(IDeviceManagerContext context)
        {
            _context = context;
        }

        public IQueryable<Group> GetGroups(Guid userId = default)
        {
            var groups = userId == Guid.Empty
                ? _context.Groups
                : _context.Groups.Where(g => g.UserId == userId);

            return groups.OrderBy(g => g.Name);
        }

        public IQueryable<Group> GetGroupById(int groupId, Guid userId = default)
        {
            var groups = userId == Guid.Empty
                ? _context.Groups
                : _context.Groups.Where(g => g.UserId == userId);

            return groups.Where(g => g.Id == groupId).Take(1);
        }

        public async Task<Group> AddAsync(Group entity, CancellationToken cancellationToken = default)
        {
            var result = await _context.Groups.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<Group> entities, CancellationToken cancellationToken = default)
        {
            await _context.Groups.AddRangeAsync(entities, cancellationToken);
        }

        public IQueryable<Group> FindAll()
        {
            return _context.Groups;
        }

        public async Task<IEnumerable<Group>> FindAsync(Expression<Func<Group, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Groups.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<Group> SingleOrDefaultAsync(Expression<Func<Group, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Groups.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(Group entity)
        {
            _context.Groups.Remove(entity);
        }

        public void RemoveRange(IEnumerable<Group> entities)
        {
            _context.Groups.RemoveRange(entities);
        }
    }
}