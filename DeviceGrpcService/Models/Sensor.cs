using System;
using System.Text.Json;

namespace DeviceGrpcService.Models
{
    public class Sensor
    {
        public int ID { get; set; }
        public int TypeID { get; set; }
        public Guid DeviceID { get; set; }

        // navigation properties
        public Device Device { get; set; }
        public SensorType Type { get; set; }
        public string Name { get; set; }
        
        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class SensorType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Unit { get; set; }
        public string UnitShort { get; set; }
        public string UnitSymbol { get; set; }
        public bool IsDiscrete { get; set; }
        public bool IsSummable { get; set; }
        
        public override string ToString() => JsonSerializer.Serialize(this);
    }
}