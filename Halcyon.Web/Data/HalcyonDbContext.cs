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
    }
}