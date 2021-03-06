using Microsoft.EntityFrameworkCore;
using post_service.Models;

namespace post_service.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Post>().ToTable("Post").HasOne(u => u.User).WithMany(p => p.Posts).HasForeignKey(p => p.UserId);
    }

}