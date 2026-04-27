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
        public DbSet<Dash> Dashes { get; set; }
        public DbSet<Block> Blocks { get; set; }

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

            modelBuilder.Entity<Dash>(eb =>
            {
                eb.HasKey(d => d.Id);
                eb.Property(d => d.Name).IsRequired().HasMaxLength(200);
                eb.Property(d => d.UserId).IsRequired();
                eb.Property(d => d.Columns).IsRequired();
                eb.Property(d => d.RowHeight).IsRequired();
                eb.Property(d => d.DisplayGrid).IsRequired();
            });

            modelBuilder.Entity<Block>(eb =>
            {
                eb.HasKey(b => b.Id);
                eb.Property(b => b.DashId).IsRequired();
                eb.Property(b => b.Text).HasMaxLength(100);
            });

        }
    }
}