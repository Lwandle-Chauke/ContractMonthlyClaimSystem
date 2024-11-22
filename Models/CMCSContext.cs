using Microsoft.EntityFrameworkCore;

namespace PROGPart2.Models
{
    public class CMCSContext : DbContext
    {
        public CMCSContext(DbContextOptions<CMCSContext> options)
            : base(options)
        {
        }

        // Define DbSets for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }

        // Configuring the model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Claim entity
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.Property(e => e.HourlyRate)
                    .HasColumnType("decimal(18, 2)"); // Specify the decimal type with precision and scale
                // You can add more configuration for other properties if needed
            });

            // Define other relationships or constraints as needed
        }
    }
}
