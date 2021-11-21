using System.Collections.Generic;
using System.Text.Json;

namespace DeviceManager.Core.Models
{
    public record SensorType
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Unit { get; set; }
        public string UnitShort { get; set; }
        public string UnitSymbol { get; set; }
        public bool IsDiscrete { get; set; }
        public bool IsSummable { get; set; }
        public IEnumerable<Sensor> Sensors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}