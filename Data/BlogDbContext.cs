using System;
using System.Text.Json;
using BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPostApi.Data;

public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
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

    base.OnModelCreating(modelBuilder);
  }

  public override int SaveChanges()
  {
    UpdateTimestamps();
    foreach (var entry in ChangeTracker.Entries<User>())
    {
      if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
      {
        var user = entry.Entity;

        // Ensure password is hashed
        if (!string.IsNullOrEmpty(user.Password)) // Check if not already hashed
        {
          user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }

        // Generate username if not provided
        if (string.IsNullOrWhiteSpace(user.UserName))
        {
          user.UserName = user.ByPassUserName ? "" : GenerateUniqueUserName(user.Email);
        }

      }
    }
    foreach (var entry in ChangeTracker.Entries<Post>())
    {
      if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
      {
        var post = entry.Entity;

        if (!string.IsNullOrWhiteSpace(post.Title))
        {
          post.Title = char.ToUpper(post.Title.Trim()[0]) + post.Title.Trim().Substring(1);
        }
      }
    }
    return base.SaveChanges();
  }

  private string GenerateUniqueUserName(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      throw new ArgumentException("Email cannot be empty when generating a username.");
    }

    string baseUserName = email.Split('@')[0].Replace(".", "").Replace("_", ""); // Remove dots & underscores
    Random random = new Random();
    string userName;

    do
    {
      int randomNumber = random.Next(1000, 9999); // Generate 4-digit number
      userName = $"{baseUserName}{randomNumber}";
    } while (Users.Any(u => u.UserName == userName)); // Ensure username is unique

    return userName;
  }

  private void UpdateTimestamps()
  {
    foreach (var entry in ChangeTracker.Entries())
    {
      if (entry.State == EntityState.Modified)
      {
        var entity = entry.Entity;

        // Check if the entity has an UpdatedAt property
        var updatedAtProperty = entity.GetType().GetProperty("UpdatedAt");
        if (updatedAtProperty != null && updatedAtProperty.PropertyType == typeof(DateTime))
        {
          updatedAtProperty.SetValue(entity, DateTime.UtcNow);
        }
      }
    }
  }

}
