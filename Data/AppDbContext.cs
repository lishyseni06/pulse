using Microsoft.EntityFrameworkCore;
using Pulse.Models;

namespace Pulse.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<News> News => Set<News>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<News>(e =>
        {
            e.ToTable("News");
            e.Property(n => n.Title).HasColumnType("nvarchar(200)");
            e.Property(n => n.Summary).HasColumnType("nvarchar(400)");
            e.Property(n => n.Content).HasColumnType("nvarchar(max)");
            e.Property(n => n.ImageUrl).HasColumnType("nvarchar(300)");
            e.HasOne(n => n.Category)
                .WithMany(c => c.News)
                .HasForeignKey(n => n.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.Property(c => c.Name).HasColumnType("nvarchar(50)");
            e.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<Admin>(e =>
        {
            e.ToTable("Admins");
            e.Property(a => a.Username).HasColumnType("nvarchar(50)");
            e.Property(a => a.PasswordHash).HasColumnType("nvarchar(64)");
            e.HasIndex(a => a.Username).IsUnique();
        });
    }
}
