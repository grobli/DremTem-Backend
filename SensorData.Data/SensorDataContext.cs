using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SensorData.Core;
using SensorData.Core.Models;
using SensorData.Data.Configurations;
using Shared.Extensions;

namespace SensorData.Data
{
    public class SensorDataContext : DbContext, ISensorDataContext
    {
        public DbSet<Reading> Readings { get; set; }

        public SensorDataContext(DbContextOptions<SensorDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConvertDateTimesToUtc();

            builder.ApplyConfiguration(new ReadingConfiguration());
        }
    }


    // for migrations to work
    public class SensorDataContextFactory : IDesignTimeDbContextFactory<SensorDataContext>
    {
        public SensorDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SensorDataContext>();
            optionsBuilder
                .UseNpgsql("Server=db-timescale; Port=5432; Database=data;", x =>
                    x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null))
                .UseSnakeCaseNamingConvention();

            return new SensorDataContext(optionsBuilder.Options);
        }
    }
}