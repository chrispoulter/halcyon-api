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
        var messages = GetMessageEvents();

        var result = base.SaveChanges();

        publishEndpoint.PublishBatch(messages).GetAwaiter().GetResult();

        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var messages = GetMessageEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        await publishEndpoint.PublishBatch(messages, cancellationToken: cancellationToken);

        return result;
    }

    private List<MessageEvent> GetMessageEvents() =>
        ChangeTracker
            .Entries()
            .Where(e =>
                e.State == EntityState.Added
                || e.State == EntityState.Modified
                || e.State == EntityState.Deleted
            )
            .SelectMany(GetMessageEvent)
            .ToList();

    private IEnumerable<MessageEvent> GetMessageEvent(EntityEntry entry)
    {
        switch (entry.Entity)
        {
            case User user:
                yield return new MessageEvent
                {
                    Id = user.Id, //TODO: FIX THIS
                    ChangeType = entry.State.ToString(),
                    Entity = nameof(User),
                };

                break;
        }
    }
}
