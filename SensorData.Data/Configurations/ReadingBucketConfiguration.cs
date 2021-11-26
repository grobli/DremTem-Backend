using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SensorData.Core.Models;

namespace SensorData.Data.Configurations
{
    public class ReadingBucketConfiguration : IEntityTypeConfiguration<ReadingBucket>
    {
        public void Configure(EntityTypeBuilder<ReadingBucket> builder)
        {
            builder
                .HasNoKey()
                .ToView("reading_buckets_view");
        }
    }
}