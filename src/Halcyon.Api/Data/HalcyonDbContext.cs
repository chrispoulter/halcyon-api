using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Data;

public class HalcyonDbContext(DbContextOptions<HalcyonDbContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(u => u.EmailAddress).IsRequired();
        modelBuilder.Entity<User>().HasIndex(u => u.EmailAddress).IsUnique();
        modelBuilder.Entity<User>().Property(u => u.FirstName).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.LastName).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.DateOfBirth).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.Roles).HasColumnType("text[]");
        modelBuilder.Entity<User>().Property(u => u.IsLockedOut).HasDefaultValue(false);
        modelBuilder.Entity<User>().Property(u => u.Version).IsRowVersion();

        modelBuilder
            .Entity<User>()
            .HasGeneratedTsVectorColumn(
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
