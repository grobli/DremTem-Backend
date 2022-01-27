using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;
using Shared.Proto;

namespace DeviceManager.Core.Models
{
    public sealed class Device : EntityBase<int, Device>
    {
        public bool Online { get; set; }

        private string _macAddress;

        public string MacAddress
        {
            get => _macAddress;
            set => _macAddress = string.IsNullOrWhiteSpace(value) ? null : NormalizeMacAddress(value);
        }

        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public DateTime? LastSeen { get; set; }
        public Location Location { get; set; }
        public int? LocationId { get; set; }
        public Guid UserId { get; init; }

        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<Group> Groups { get; set; }

        public Device()
        {
        }

        /** copy constructor */
        public Device(Device originalDevice) : base(originalDevice)
        {
            Online = originalDevice.Online;
            MacAddress = originalDevice.MacAddress;
            Model = originalDevice.Model;
            Manufacturer = originalDevice.Manufacturer;
            LastSeen = originalDevice.LastSeen;
            LocationId = originalDevice.LocationId;
            UserId = originalDevice.UserId;
        }

        public override void MapEditableFields(Device source)
        {
            base.MapEditableFields(source);

            Online = source.Online;
            MacAddress = source.MacAddress;
            Model = source.Model;
            Manufacturer = source.Manufacturer;
            LocationId = source.LocationId;
        }

        private static string NormalizeMacAddress(string mac)
        {
            return string.Join(":",
                Regex.Replace(mac.ToUpper(), @"[:\.-]", "")
                    .Select((ch, i) => new { Character = ch, Index = i / 2 })
                    .GroupBy(x => x.Index, x => x.Character)
                    .Select(x => string.Concat(x)));
        }
    }

    public class DeviceParameters : QueryStringParametersBase
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

        public IEnumerable<Entity> FieldsToInclude() => _fieldsToInclude;
    }
}