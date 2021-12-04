using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using DeviceManager.Data.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IValidator<Group> _validator;
        private IValidator<Group> Validator => _validator ??= new GroupValidator(_unitOfWork);

        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Group> GetAllGroupsQuery(Guid userId = default)
        {
            return _unitOfWork.Groups.GetGroups(userId);
        }

        public IQueryable<Group> GetGroupQuery(int groupId, Guid userId = default)
        {
            return _unitOfWork.Groups.GetGroupById(groupId, userId);
        }

        public async Task<Group> CreateGroupAsync(Group newGroup, CancellationToken cancellationToken = default)
        {
            newGroup.Created = DateTime.UtcNow;
            newGroup.Name = newGroup.Name.Trim();
            newGroup.DisplayName = string.IsNullOrWhiteSpace(newGroup.DisplayName) ? null : newGroup.DisplayName.Trim();
            await Validator.ValidateAndThrowAsync(newGroup, cancellationToken);

            await _unitOfWork.Groups.AddAsync(newGroup, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newGroup;
        }

        public async Task UpdateGroupAsync(Group groupToBeUpdated, Group group,
            CancellationToken cancellationToken = default)
        {
            var backup = groupToBeUpdated with { };

            groupToBeUpdated.LastModified = DateTime.UtcNow;
            groupToBeUpdated.DisplayName = string.IsNullOrWhiteSpace(group.DisplayName)
                ? null
                : group.DisplayName.Trim();

            var validationResult = await Validator.ValidateAsync(groupToBeUpdated, cancellationToken);
            if (!validationResult.IsValid)
            {
                Restore();
                throw new ValidationException(validationResult.Errors);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            void Restore()
            {
                groupToBeUpdated.LastModified = backup.LastModified;
                groupToBeUpdated.DisplayName = backup.DisplayName;
            }
        }

        public async Task DeleteGroupAsync(Group @group, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Groups.Remove(group);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task AddDevice(Group group, Device device, CancellationToken cancellationToken = default)
        {
            group = await _unitOfWork.Groups.GetGroupById(@group.Id).Include(g => g.Devices)
                .SingleOrDefaultAsync(cancellationToken);
            group.Devices.Add(device);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task RemoveDevice(Group group, Device device, CancellationToken cancellationToken = default)
        {
            group = await _unitOfWork.Groups.GetGroupById(group.Id).Include(g => g.Devices)
                .SingleOrDefaultAsync(cancellationToken);
            group.Devices.Remove(device);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}