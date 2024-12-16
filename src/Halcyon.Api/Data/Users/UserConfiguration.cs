using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Halcyon.Api.Data.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.EmailAddress).IsRequired();
        builder.HasIndex(u => u.EmailAddress).IsUnique();
        builder.Property(u => u.FirstName).IsRequired();
        builder.Property(u => u.LastName).IsRequired();
        builder.Property(u => u.DateOfBirth).IsRequired();
        builder.Property(u => u.Roles).HasColumnType("text[]");
        builder.Property(u => u.IsLockedOut).HasDefaultValue(false);
        builder.Property(u => u.Version).IsRowVersion();

        builder.HasGeneratedTsVectorColumn(
            u => u.SearchVector,
            "english",
            u => new
            {
                u.FirstName,
                u.LastName,
                u.EmailAddress,
            }
        );
    }
}
