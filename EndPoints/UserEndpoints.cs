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

    return group;
  }

  
}
