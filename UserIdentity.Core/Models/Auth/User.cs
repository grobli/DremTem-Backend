using System;
using Microsoft.AspNetCore.Identity;
using Shared;

namespace UserIdentity.Core.Models.Auth
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserPagedParameters
    {
        public PaginationParameters Page { get; } = new();
    }
}