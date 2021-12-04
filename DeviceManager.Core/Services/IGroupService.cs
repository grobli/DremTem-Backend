using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Services
{
    public interface IGroupService
    {
        IQueryable<Group> GetAllGroupsQuery(Guid userId = default);
        IQueryable<Group> GetGroupQuery(int groupId, Guid userId = default);

        Task<Group> CreateGroupAsync(Group newGroup, CancellationToken cancellationToken = default);

        Task UpdateGroupAsync(Group groupToBeUpdated, Group group,
            CancellationToken cancellationToken = default);

        Task DeleteGroupAsync(Group group, CancellationToken cancellationToken = default);

        Task AddDevice(Group group, Device device, CancellationToken cancellationToken = default);

        Task RemoveDevice(Group group, Device device, CancellationToken cancellationToken = default);
    }
}