using Shared.Proto;

namespace DeviceManager.Core.Messages
{
    public class CreatedSensorTypeMessage
    {
        public CreatedSensorTypeMessage(SensorTypeDto createdSensorType)
        {
            CreatedSensorType = createdSensorType;
        }

        public SensorTypeDto CreatedSensorType { get; set; }
    }

    public class UpdatedSensorTypeMessage
    {
        public UpdatedSensorTypeMessage(SensorTypeDto updatedSensorType)
        {
            UpdatedSensorType = updatedSensorType;
        }

        public SensorTypeDto UpdatedSensorType { get; set; }
    }

    public class DeletedSensorTypeMessage
    {
        public DeletedSensorTypeMessage(int deletedSensorTypeId)
        {
            DeletedSensorTypeId = deletedSensorTypeId;
        }

        public int DeletedSensorTypeId { get; set; }
    }
}