using System;
using System.Text.Json;

namespace DeviceManager.Core.Models
{
    public record Sensor
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public long DeviceId { get; set; }
        public Device Device { get; set; }
        public string TypeName { get; set; }
        public SensorType Type { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}