using System;
using DeviceManager.Core.Proto;

namespace DeviceManager.Core.Extensions
{
    public static class GenericDeleteRequestExtension
    {
        public static Guid UserId(this GenericDeleteRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }
    }
}