using Microsoft.EntityFrameworkCore;
using SensorData.Core.Models;
using SensorData.Data.Configurations;
using Shared.Extensions;

namespace SensorData.Data
{
    public class SensorDataContext : DbContext
    {
        public DbSet<Reading> Readings { get; set; }
        public DbSet<ReadingBucket> ReadingBuckets { get; set; }

        public SensorDataContext(DbContextOptions<SensorDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.ConvertDateTimesToUtc();

            builder.ApplyConfiguration(new ReadingConfiguration());
            builder.ApplyConfiguration(new ReadingBucketConfiguration());
        }
    }
}