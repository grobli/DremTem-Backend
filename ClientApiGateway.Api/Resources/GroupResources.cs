using System;
using System.Collections.Generic;

namespace ClientApiGateway.Api.Resources
{
    public record CreateGroupResource(string Name, string DisplayName);

    public record UpdateGroupResource(string DisplayName);

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