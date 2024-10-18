using Microsoft.EntityFrameworkCore;
using PROGPart2.Models;

namespace PROGPart2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Claim> Claims { get; set; }

        // Other DbSets for your models (e.g., Users, Roles, etc.)
    }
}
