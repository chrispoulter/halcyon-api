using Halcyon.Api.Data.Users;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Data;

public class HalcyonDbContext(DbContextOptions<HalcyonDbContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HalcyonDbContext).Assembly);
    }
}
