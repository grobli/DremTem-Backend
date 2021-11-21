using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DeviceManager.Core.Models
{
    public record Device
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ApiKey { get; set; }
        public bool Online { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public Location Location { get; set; }
        public string LocationName { get; set; }
        public Guid UserId { get; set; }

        public ICollection<Sensor> Sensors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}