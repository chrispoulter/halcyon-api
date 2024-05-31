using Halcyon.Api.Features.Messaging;
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

    public override int SaveChanges()
    {
        var changedEntries = GetChangedEntries();

        var result = base.SaveChanges();

        PublishEvents(changedEntries).GetAwaiter().GetResult();

        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changedEntries = GetChangedEntries();

        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishEvents(changedEntries);

        return result;
    }

    private List<ChangedEntry> GetChangedEntries() =>
        ChangeTracker
            .Entries()
            .Where(e =>
                e.State == EntityState.Added
                || e.State == EntityState.Modified
                || e.State == EntityState.Deleted
            )
            .Select(e => new ChangedEntry { Entity = e.Entity, State = e.State })
            .ToList();

    private async Task PublishEvents(List<ChangedEntry> changedEntries)
    {
        foreach (var entry in changedEntries)
        {
            switch (entry.Entity)
            {
                case User user:
                    var message = new MessageEvent
                    {
                        Id = user.Id,
                        ChangeType = entry.State.ToString(),
                        Entity = nameof(User)
                    };

                    await publishEndpoint.Publish(message);
                    break;
            }
        }
    }

    private class ChangedEntry
    {
        public object Entity { get; set; }

        public EntityState State { get; set; }
    }
}
