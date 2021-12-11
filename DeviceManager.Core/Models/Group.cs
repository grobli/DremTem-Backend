using System;
using System.Collections.Generic;
using Shared;

namespace DeviceManager.Core.Models
{
    public record Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public Guid UserId { get; set; } // owner
        public ICollection<Device> Devices { get; set; }
    }
}