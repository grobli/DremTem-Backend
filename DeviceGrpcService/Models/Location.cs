using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace DeviceGrpcService.Models
{
    [Index(nameof(CreatedById))]
    public class Location
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LastModified { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? Created { get; set; }

        [Required] public Guid CreatedById { get; set; }

        public ICollection<Device> Devices { get; set; }


        public override string ToString() => JsonSerializer.Serialize(this);
    }
}