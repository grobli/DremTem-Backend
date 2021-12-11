using System;
using System.Collections.Generic;
using Shared;
using Shared.Proto;

namespace DeviceManager.Core.Models
{
    public sealed class Location : EntityBase<int, Location>
    {
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public Guid UserId { get; init; }

        public ICollection<Device> Devices { get; set; }

        public Location()
        {
        }

        /** copy constructor */
        public Location(Location originalLocation) : base(originalLocation)
        {
            Latitude = originalLocation.Latitude;
            Longitude = originalLocation.Longitude;
            UserId = originalLocation.UserId;
        }

        public override void MapEditableFields(Location source)
        {
            base.MapEditableFields(source);

            Latitude = source.Latitude;
            Longitude = source.Longitude;
        }
    }

    public class LocationParameters : QueryStringParametersBase
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
}