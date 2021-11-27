using System;
using System.Collections.Generic;
using System.Linq;
using DeviceManager.Core.Proto;

namespace DeviceManager.Core.Extensions
{
    public static class GetRequestParametersExtension
    {
        public static HashSet<Entity> IncludeFieldsSet(this GetRequestParameters self, params Entity[] filterFields)
        {
            return self.IncludeFields
                .Distinct()
                .Where(filterFields.Contains)
                .ToHashSet();
        }

        public static Guid UserId(this GetRequestParameters self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }
    }
}