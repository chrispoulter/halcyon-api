using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Data;

public class HalcyonDbContext(DbContextOptions<HalcyonDbContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();
        user.Property(u => u.Id).HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        user.Property(u => u.EmailAddress).IsRequired();
        user.HasIndex(u => u.EmailAddress).IsUnique();
        user.Property(u => u.FirstName).IsRequired();
        user.Property(u => u.LastName).IsRequired();
        user.Property(u => u.DateOfBirth).IsRequired();
        user.Property(u => u.IsLockedOut).HasDefaultValue(false);
        user.Property(u => u.Version).IsRowVersion();
    }
}
