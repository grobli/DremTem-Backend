using System;
using System.Collections.Generic;

namespace ClientApiGateway.Api.Resources.Group
{
    public class GroupResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<int> DeviceIds { get; set; }
    }
}