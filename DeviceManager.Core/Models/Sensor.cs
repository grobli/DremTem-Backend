using System;
using System.Text.Json;

namespace DeviceManager.Core.Models
{
    public record Sensor
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DeviceName { get; set; }
        public Device Device { get; set; }
        public string TypeName { get; set; }
        public SensorType Type { get; set; }
        public Guid UserId { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}