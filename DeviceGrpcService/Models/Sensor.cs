using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DeviceGrpcService.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid DeviceId { get; set; }
        public Device Device { get; set; }

        public int TypeId { get; set; }
        public SensorType Type { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class SensorType
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string DataType { get; set; }
        [Required] public string Unit { get; set; }
        [Required] public string UnitShort { get; set; }
        public string UnitSymbol { get; set; }
        public bool IsDiscrete { get; set; }
        public bool IsSummable { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}