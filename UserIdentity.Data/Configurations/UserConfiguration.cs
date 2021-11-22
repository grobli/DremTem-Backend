using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public const int FirstNameMaxLength = 100;
        public const int LastNameMaxLength = 100;

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(u => u.FirstName)
                .HasMaxLength(FirstNameMaxLength);

            builder
                .Property(u => u.LastName)
                .HasMaxLength(LastNameMaxLength);
        }
    }
}