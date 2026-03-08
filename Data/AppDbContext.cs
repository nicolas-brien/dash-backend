using Microsoft.EntityFrameworkCore;
using DashBackend.Models;

namespace DashBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Network> Networks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Network>(eb =>
            {
                eb.HasKey(n => n.Id);
                eb.Property(n => n.Name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<User>(eb =>
            {
                eb.HasKey(u => u.Id);
                eb.Property(u => u.Username).IsRequired().HasMaxLength(100);
            });
        }
    }
}