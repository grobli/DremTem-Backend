using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserGrpcService.Models;
using UserGrpcService.Models.Auth;

namespace UserGrpcService.Data
{
    public class UserDbContext : IdentityDbContext<User, Role, Guid>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Email);
        }
    }
}