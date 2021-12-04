using System;
using System.Linq;
using DeviceManager.Core.Models;
using Shared.Repositories;

namespace DeviceManager.Core.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        IQueryable<Group> GetGroups(Guid userId = default);
        IQueryable<Group> GetGroupById(int groupId, Guid userId = default);
    }
}