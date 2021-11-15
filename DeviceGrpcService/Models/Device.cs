using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace DeviceGrpcService.Models
{
    [Index(nameof(OwnerId), nameof(LocationId))]
    public class Device
    {
        [Key] public Guid Id { get; set; }
        public string Name { get; set; }

        [Required] [MaxLength(128)] public string ApiKey { get; set; }
        public bool Online { get; set; }
        public DateTime? LastSeen { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LastModified { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? Created { get; set; }


        [ForeignKey(nameof(LocationId))] public Location Location { get; set; }
        public int? LocationId { get; set; }

        [Required] public Guid OwnerId { get; set; }

        public ICollection<Sensor> Sensors { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}