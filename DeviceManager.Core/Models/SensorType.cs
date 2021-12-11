using System.Collections.Generic;

namespace DeviceManager.Core.Models
{
    public sealed class SensorType : EntityBase<int, SensorType>
    {
        public string Unit { get; set; }
        public string UnitShort { get; set; }
        public string UnitSymbol { get; set; }
        public bool IsDiscrete { get; set; }
        public bool IsSummable { get; set; }
        public IEnumerable<Sensor> Sensors { get; set; }

        public SensorType()
        {
        }

        /** copy constructor */
        public SensorType(SensorType originalType) : base(originalType)
        {
            Unit = originalType.Unit;
            UnitShort = originalType.UnitShort;
            UnitSymbol = originalType.UnitSymbol;
            IsDiscrete = originalType.IsDiscrete;
            IsSummable = originalType.IsSummable;
        }

        public override void MapEditableFields(SensorType source)
        {
            base.MapEditableFields(source);

            Unit = source.Unit;
            UnitShort = source.UnitShort;
            UnitSymbol = source.UnitSymbol;
            IsDiscrete = source.IsDiscrete;
            IsSummable = source.IsSummable;
        }
    }
}