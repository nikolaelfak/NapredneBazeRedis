using Microsoft.EntityFrameworkCore;

namespace FitAndFun.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Routine> Routines { get; set; }
        public DbSet<Cilj> Cilj { get; set; }
        public DbSet<Nagrade> Nagrade { get; set; }
    }
}
