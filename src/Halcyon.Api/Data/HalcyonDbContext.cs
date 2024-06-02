using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Halcyon.Api.Data;

public class HalcyonDbContext : DbContext
{
    private readonly IPublishEndpoint publishEndpoint;

    public HalcyonDbContext() { }

    public HalcyonDbContext(
        DbContextOptions<HalcyonDbContext> options,
        IPublishEndpoint publishEndpoint
    )
        : base(options)
    {
        this.publishEndpoint = publishEndpoint;

        ChangeTracker.StateChanged += StateChanged;
    }

    private void StateChanged(object sender, EntityStateChangedEventArgs e)
    {
        if (e.OldState == EntityState.Unchanged)
        {
            return;
        }

        if (e.Entry.Entity is not IEntityWithId entity)
        {
            return;
        }

        var message = new EntityChangedEvent
        {
            Id = entity.Id,
            ChangeType = e.OldState,
            Entity = entity.GetType().Name
        };

        publishEndpoint.Publish(message).GetAwaiter().GetResult();
    }

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
