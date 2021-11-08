using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DeviceGrpcService.Models
{
    public class Device
    {
        public Guid ID { get; set; }
        public int? LocationID { get; set; }
        public string Name { get; set; }
        public bool Online { get; set; }

        // navigation properties
        public Location Location { get; set; }
        public ICollection<Sensor> Sensors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}