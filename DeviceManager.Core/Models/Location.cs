using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DeviceManager.Core.Models
{
    public record Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }

        public ICollection<Device> Devices { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}