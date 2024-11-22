using Microsoft.EntityFrameworkCore;

namespace PROGPart2.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Ensure this matches your User class

        public DbSet<Claim> Claims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users"); // Maps User to dbo.Users
            modelBuilder.Entity<Claim>().ToTable("Claims"); // Maps Claim to dbo.Claims
            modelBuilder.Entity<Claim>()
                .Ignore(c => c.ClaimDocument);
        }

    }
}
