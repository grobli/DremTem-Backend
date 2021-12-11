using System;
using System.Collections.Generic;
using System.Text.Json;
using Shared;
using Shared.Proto.Common;

namespace DeviceManager.Core.Models
{
    public record Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public Guid UserId { get; set; }

        public ICollection<Device> Devices { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }

    public class LocationParameters : QueryStringParameters
    {
        private readonly List<Entity> _fieldsToInclude = new();
        private bool _includeDevices;

        public bool IncludeDevices
        {
            get => _includeDevices;
            set
            {
                if (value) _fieldsToInclude.Add(Entity.Device);
                _includeDevices = value;
            }
        }

        public IReadOnlyCollection<Entity> FieldsToInclude() => _fieldsToInclude;
    }

    public class LocationPagedParameters : LocationParameters
    {
        public PaginationParameters Page { get; }= new();
    }
}