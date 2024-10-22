﻿using System;
using Shared.Proto;

namespace Shared.Extensions
{
    public static class UpdateRequestExtensions
    {
        public static Guid UserId(this UpdateDeviceRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }

        public static Guid UserId(this UpdateLocationRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }

        public static Guid UserId(this UpdateSensorRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }

        public static Guid UserId(this UpdateGroupRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }
    }
}