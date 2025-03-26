using System;
using System.Text.Json;
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

    if (!string.IsNullOrWhiteSpace(updateUser.UserName))
    {
      user.UserName = updateUser.UserName;
    }

    // Store the original name before updating
    if (!string.IsNullOrWhiteSpace(updateUser.Name))
    {
      var oldName = user.Name;

      var history = string.IsNullOrWhiteSpace(user.NameHistory)
      ? new List<Dictionary<string, string>>() // Initialize empty list if history is empty
      : JsonSerializer.Deserialize<List<Dictionary<string, string>>>(user.NameHistory) ?? new List<Dictionary<string, string>>();

      if (!string.IsNullOrWhiteSpace(oldName) && oldName != updateUser.Name)
      {

        if (!history.Any(h => h["name"] == oldName))
        {
          history.Add(new Dictionary<string, string>
              {
                { "name", oldName },
                { "createdAt", DateTime.UtcNow.ToString("o") } // ISO 8601 format
              });
        }

        user.Name = updateUser.Name;

        // Serialize back to JSON and update NameHistory field
        user.NameHistory = JsonSerializer.Serialize(history);
      }
    }
    // Save changes to the database
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
  });


    group.MapPatch("/{id}/revert", (int id, BlogDbContext dbContext) =>
{
  var user = dbContext.Users.Find(id);
  if (user == null)
  {
    return Results.NotFound("User not found.");
  }

  var history = user.NameHistoryList;
  if (history == null || history.Count == 0)
  {
    return Results.BadRequest("No previous name to revert.");
  }

  // Get the last name from history
  var lastEntry = history.Last();
  user.Name = lastEntry["name"];

  // Remove the last entry
  history.RemoveAt(history.Count - 1);

  user.NameHistory = JsonSerializer.Serialize(history);

  Console.WriteLine("i am in here {}");

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


    group.MapDelete("/{id}", (int id, BlogDbContext dbContext) =>
  {
    try
    {
      dbContext.SaveChanges();
      return Results.Ok($"User {id} and their posts have been soft deleted.");
    }
    catch (DbUpdateException ex)
    {
      return Results.Problem($"Error deleting user: {ex.InnerException?.Message ?? ex.Message}");
    }
  });
      return group;
    }
  }
