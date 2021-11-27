using System;
using DeviceManager.Core.Proto;

namespace DeviceManager.Core.Extensions
{
    public static class GenerateTokenRequestExtension
    {
        public static Guid UserId(this GenerateTokenRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }
    }
}