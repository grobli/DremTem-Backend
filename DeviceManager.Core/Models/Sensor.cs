using System.Collections.Generic;
using Shared;
using Shared.Proto;

namespace DeviceManager.Core.Models
{
    public sealed class Sensor : EntityBase<int, Sensor>
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public int TypeId { get; set; }
        public SensorType Type { get; set; }

        public Sensor()
        {
        }

        /** copy constructor */
        public Sensor(Sensor originalSensor) : base(originalSensor)
        {
            DeviceId = originalSensor.DeviceId;
            TypeId = originalSensor.TypeId;
        }

        public override void MapEditableFields(Sensor source)
        {
            base.MapEditableFields(source);

            TypeId = source.TypeId;
        }
    }

    public class SensorParameters : QueryStringParametersBase
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
}