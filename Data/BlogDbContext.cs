using BlogPostApi.Data.Interceptors;
using BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlogPostApi.Data;

public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
  private readonly UserInterceptor _userInterceptor = new();
  private readonly PostInterceptor _postInterceptor = new();
  public DbSet<User> Users { get; set; }

  public DbSet<Post> Posts { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>()
           .HasIndex(u => u.UserName)
           .IsUnique();

    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

    modelBuilder.Entity<Post>()
        .HasOne(p => p.User)
        .WithMany(u => u.Posts)
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, their posts will also be deleted

    modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

    modelBuilder.Entity<Post>().HasQueryFilter(p => !p.IsDeleted);

    base.OnModelCreating(modelBuilder);
  }

  public override int SaveChanges()
  {
    ApplyInterceptors(ChangeTracker);
    ApplyTimestamps();
    return base.SaveChanges();
  }

  private void ApplyInterceptors(ChangeTracker changeTracker, bool hardDelete = false)
  {
    foreach (var entry in changeTracker.Entries())
    {
      switch (entry.Entity)
      {
        case User user:
          _userInterceptor.Apply((EntityEntry<User>)entry, hardDelete);
          break;
        case Post post:
          _postInterceptor.Apply((EntityEntry<Post>)entry, hardDelete);
          break;
      }
    }
  }

  private void ApplyTimestamps()
  {
    foreach (var entry in ChangeTracker.Entries())
    {
      if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
      {
        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
      }
    }
  }

}
