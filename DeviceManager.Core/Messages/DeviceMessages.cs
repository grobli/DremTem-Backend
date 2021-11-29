using System.Collections.Generic;
using DeviceManager.Core.Proto;

namespace DeviceManager.Core.Messages
{
    public class CreatedDeviceMessage
    {
        public DeviceExtendedDto CreatedDevice { get; set; }

        public CreatedDeviceMessage(DeviceExtendedDto createdDevice)
        {
            CreatedDevice = createdDevice;
        }
    }

    public class DeletedDeviceMessage
    {
        public int DeletedDeviceId { get; set; }
        public List<int> DeletedSensorIds { get; set; }

        public DeletedDeviceMessage(int deletedDeviceId, List<int> deletedSensorIds)
        {
            DeletedDeviceId = deletedDeviceId;
            DeletedSensorIds = deletedSensorIds;
        }
    }

    public class UpdatedDeviceMessage
    {
        public DeviceDto UpdatedDevice { get; set; }

        public UpdatedDeviceMessage(DeviceDto updatedDevice)
        {
            UpdatedDevice = updatedDevice;
        }
    }
}