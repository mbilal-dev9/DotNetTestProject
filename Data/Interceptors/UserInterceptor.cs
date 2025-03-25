using System;
using BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BlogPostApi.Data.Interceptors;

public class UserInterceptor
{
  public void Apply(EntityEntry<User> entry, bool hardDelete = false)
  {
    var user = entry.Entity;
    switch (entry.State)
    {
      case EntityState.Added:
        SaveOrUpdate(user);
        Save(user);
        break;
      case EntityState.Modified:
        SaveOrUpdate(user);
        Update(user);
        break;
      case EntityState.Deleted:
        Delete(user, hardDelete);
        break;
    }
  }

  public void SaveOrUpdate(User user)  // make this function to check if the user ByPassUserName if not then create user name 
  {
    user.UserName = user.ByPassUserName ? "" : GenerateUniqueUserName(user.Email);
    if (!string.IsNullOrEmpty(user.Password)) // Check if not already hashed
    {
      user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    }
  }
  public void Save(User user) { }
  public void Update(User user) { }
  public void Delete(User user, bool hardDelete = false)
  {
    if (hardDelete == false)
    {
      user.IsDeleted = true;

      foreach (var post in user.Posts)
      {
        post.IsDeleted = true;
      }
    }

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

    int randomNumber = random.Next(1000, 9999); // Generate 4-digit number
    userName = $"{baseUserName}{randomNumber}";

    return userName;
  }

}
