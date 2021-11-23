using System;
using Microsoft.AspNetCore.Identity;

namespace UserIdentity.Core.Models.Auth
{
    public class Role : IdentityRole<Guid>
    {
    }

    public static class DefaultRoles
    {
        public const string BaseUser = "BaseUser";
        public const string SuperUser = "SuperUser";
    }
}