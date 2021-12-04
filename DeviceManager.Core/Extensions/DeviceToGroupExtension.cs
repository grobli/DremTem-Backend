using System;
using DeviceManager.Core.Proto;

namespace DeviceManager.Core.Extensions
{
    public static class DeviceToGroupExtension
    {
        public static Guid UserId(this AddDeviceToGroupRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }

        public static Guid UserId(this RemoveDeviceFromGroupRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }
    }
}