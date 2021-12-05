using Shared.Proto;

namespace DeviceManager.Core.Messages
{
    public class CreatedSensorMessage
    {
        public CreatedSensorMessage(SensorDto createdSensor)
        {
            CreatedSensor = createdSensor;
        }

        public SensorDto CreatedSensor { get; set; }
    }

    public class DeletedSensorMessage
    {
        public DeletedSensorMessage(int deletedSensorId, int parentDeviceId)
        {
            DeletedSensorId = deletedSensorId;
            ParentDeviceId = parentDeviceId;
        }

        public int DeletedSensorId { get; set; }
        public int ParentDeviceId { get; set; }
    }

    public class UpdatedSensorMessage
    {
        public UpdatedSensorMessage(SensorDto updatedSensor)
        {
            UpdatedSensor = updatedSensor;
        }

        public SensorDto UpdatedSensor { get; set; }
    }
}