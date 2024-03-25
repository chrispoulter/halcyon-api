using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Data;

public class HalcyonDbContext : DbContext
{
    public HalcyonDbContext() { }

    public HalcyonDbContext(DbContextOptions<HalcyonDbContext> options)
        : base(options) { }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_trgm");

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
            .Property(u => u.Search)
            .HasComputedColumnSql(
                @"""first_name"" || ' ' || ""last_name"" || ' ' || ""email_address""",
                stored: true
            );

        modelBuilder
            .Entity<User>()
            .HasIndex(u => u.Search)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
    }
}
