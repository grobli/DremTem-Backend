using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Data
{
    public class UserDbContext : IdentityDbContext<User, Role, Guid>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
    }
}