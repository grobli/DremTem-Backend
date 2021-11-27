using System;
using System.Collections.Generic;
using System.Text.Json;
using Shared;

namespace DeviceManager.Core.Models
{
    public record SensorType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Unit { get; set; }
        public string UnitShort { get; set; }
        public string UnitSymbol { get; set; }
        public bool IsDiscrete { get; set; }
        public bool IsSummable { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<Sensor> Sensors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class SensorTypeParameters : QueryStringParameters
    {
    }

    public class SensorTypePagedParameters : SensorParameters
    {
        public PageQueryStringParameters Page { get; } = new();
    }
}