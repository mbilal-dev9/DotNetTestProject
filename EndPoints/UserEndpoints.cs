using System;
using BlogPostApi.Data;
using BlogPostApi.DTOs;
using BlogPostApi.Mapping;
using BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPostApi.EndPoints;

public static class UserEndpoints
{
  const string GetUserEndPointName = "getUser";

  public static RouteGroupBuilder MapUserEndPoints(this WebApplication app)
  {
    var group = app.MapGroup("users").WithParameterValidation();

    group.MapGet("/", (BlogDbContext dbContext) =>
        dbContext.Users
        .Select(user => user.ToUserSummaryDto())
        .AsNoTracking()
    );

    group.MapGet("/{id}", (int id, BlogDbContext dbContext) =>
   {
     User? user = dbContext.Users.Find(id);
     return user is null ? Results.NotFound() : Results.Ok(user.ToUserSummaryDto());

   }).WithName(GetUserEndPointName);

    group.MapPost("/", (UserCreateDto newUser, BlogDbContext dbContext) =>
    {
      if (dbContext.Users.Any(u => u.Email == newUser.Email))
      {
        return Results.BadRequest("A user with this email already exists.");
      }

      User user = newUser.ToEntity();

      dbContext.Users.Add(user);
      try
      {
        dbContext.SaveChanges();
      }
      catch (DbUpdateException ex)
      {
        return Results.Problem($"An error occurred while saving the user: {ex.InnerException?.Message ?? ex.Message}");
      }

      return Results.CreatedAtRoute(
         GetUserEndPointName,
         new { id = user.Id },
         user.ToUserSummaryDto());
    }
    );

    group.MapPatch("/{id}", (int id, UpdateUserDto updateUser, BlogDbContext dbContext) =>
   {
     User? user = dbContext.Users.Find(id);

     if (user == null)
     {
       return Results.NotFound($"User with ID {id} not found.");
     }
     if (!string.IsNullOrWhiteSpace(updateUser.Name))
     {
       user.Name = updateUser.Name;
     }

     if (!string.IsNullOrWhiteSpace(updateUser.UserName))
     {
       user.UserName = updateUser.UserName;
     }

     dbContext.Users.Update(user);
     try
     {
       dbContext.SaveChanges();
     }
     catch (DbUpdateException ex)
     {
       return Results.Problem($"An error occurred while saving the user: {ex.InnerException?.Message ?? ex.Message}");
     }
      return Results.Ok(user.ToUserSummaryDto());
   }
   );

    group.MapPatch("/{id}/revert", (int id, BlogDbContext dbContext) =>
{
    var user = dbContext.Users.Find(id);
    if (user == null)
    {
        return Results.NotFound("User not found.");
    }

    var history = user.NameHistoryList;
    if (history.Count == 0)
    {
        return Results.BadRequest("No previous name to revert.");
    }

    // Get last name object and extract the name
    var lastNameEntry = history[^1];
    var lastName = lastNameEntry["name"];

    history.RemoveAt(history.Count - 1); // Remove last entry

    user.Name = lastName;
    user.NameHistoryList = history; // Save the updated history

    try
    {
        dbContext.SaveChanges();
        return Results.Ok(user.ToUserSummaryDto());
    }
    catch (DbUpdateException ex)
    {
        return Results.Problem($"Error reverting username: {ex.InnerException?.Message ?? ex.Message}");
    }
});
    return group;
  }


}
