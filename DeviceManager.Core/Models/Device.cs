using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DeviceManager.Core.Models
{
    public record Device
    {
        private string _macAddress;

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Online { get; set; }

        public string MacAddress
        {
            get => _macAddress;
            set => _macAddress = NormalizeMacAddress(value);
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
}