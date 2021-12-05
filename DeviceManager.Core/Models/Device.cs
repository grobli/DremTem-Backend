using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Shared;
using Shared.Proto.Common;

namespace DeviceManager.Core.Models
{
    public record Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Online { get; set; }

        private string _macAddress;

        public string MacAddress
        {
            get => _macAddress;
            set => _macAddress = value is null ? null : NormalizeMacAddress(value);
        }

        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public Location Location { get; set; }
        public int? LocationId { get; set; }
        public Guid UserId { get; set; }

        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<Group> Groups { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);

        private static string NormalizeMacAddress(string mac)
        {
            return string.Join(":",
                Regex.Replace(mac.ToUpper(), @"[:\.-]", "")
                    .Select((ch, i) => new { Character = ch, Index = i / 2 })
                    .GroupBy(x => x.Index, x => x.Character)
                    .Select(x => string.Concat(x)));
        }
    }

    public class DeviceParameters : QueryStringParameters
    {
        private readonly List<Entity> _fieldsToInclude = new();
        private bool _includeLocation;
        private bool _includeSensors;

        public bool IncludeLocation
        {
            get => _includeLocation;
            set
            {
                if (value) _fieldsToInclude.Add(Entity.Location);
                _includeLocation = value;
            }
        }

        public bool IncludeSensors
        {
            get => _includeSensors;
            set
            {
                if (value) _fieldsToInclude.Add(Entity.Sensor);
                _includeSensors = value;
            }
        }

        public IReadOnlyCollection<Entity> FieldsToInclude() => _fieldsToInclude;
    }

    public class DevicePagedParameters : DeviceParameters
    {
        public PageQueryStringParameters Page { get; } = new();
    }
}