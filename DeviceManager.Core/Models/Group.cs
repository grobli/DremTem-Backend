using System;
using System.Collections.Generic;

namespace DeviceManager.Core.Models
{
    public sealed class Group : EntityBase<int, Group>
    {
        public Guid UserId { get; init; } // owner
        public ICollection<Device> Devices { get; set; }

        public Group()
        {
        }

        /** copy constructor */
        public Group(Group originalGroup) : base(originalGroup)
        {
            UserId = originalGroup.UserId;
        }
    }
}