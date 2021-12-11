using System;
using System.Collections.Generic;
using System.Text.Json;
using Shared;
using Shared.Proto.Common;

namespace DeviceManager.Core.Models
{
    public record Sensor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public int TypeId { get; set; }
        public SensorType Type { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class SensorParameters : QueryStringParameters
    {
        private readonly List<Entity> _fieldsToInclude = new();
        private bool _includeType;

        public bool IncludeType
        {
            get => _includeType;
            set
            {
                if (value) _fieldsToInclude.Add(Entity.SensorType);
                _includeType = value;
            }
        }

        public IReadOnlyCollection<Entity> FieldsToInclude() => _fieldsToInclude;
    }

    public class SensorPagedParameters : SensorParameters
    {
        public PaginationParameters Page { get; } = new();
    }
}