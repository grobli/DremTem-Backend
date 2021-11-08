using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DeviceGrpcService.Models
{
    public class Location
    {
        public int ID { get; set; }
        [Required] public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}