using System;
using BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPostApi.Data;

public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>()
           .HasIndex(u => u.UserName)
           .IsUnique();

    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

    base.OnModelCreating(modelBuilder);
  }

  public override int SaveChanges()
  {
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
}
