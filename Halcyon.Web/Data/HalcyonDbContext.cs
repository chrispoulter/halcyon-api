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

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
              .HasIndex(u => u.EmailAddress)
              .IsUnique();

            modelBuilder.Entity<Role>()
              .HasIndex(r => r.Name)
              .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.RoleId, ur.UserId });
        }
    }
}