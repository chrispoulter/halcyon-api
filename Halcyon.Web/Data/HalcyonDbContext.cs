using Microsoft.EntityFrameworkCore;

namespace Halcyon.Web.Data
{
    public class HalcyonDbContext : DbContext
    {
        public HalcyonDbContext(DbContextOptions<HalcyonDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
              .HasIndex(u => u.EmailAddress)
              .IsUnique();

            modelBuilder.Entity<User>()
              .Property(u => u.Roles)
              .HasColumnType("text[]");
        }
    }
}