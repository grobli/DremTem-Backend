using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DeviceManager.Core.Models
{
    public record Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Online { get; set; }
        public string MacAddress { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public Location Location { get; set; }
        public int? LocationId { get; set; }
        public Guid UserId { get; set; }

        public ICollection<Sensor> Sensors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}