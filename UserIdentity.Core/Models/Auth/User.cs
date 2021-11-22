using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UserIdentity.Core.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}