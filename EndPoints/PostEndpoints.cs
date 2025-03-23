using BlogPostApi.Data;
using BlogPostApi.DTOs;
using BlogPostApi.Mapping;
using BlogPostAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogPostApi.EndPoints;

public static class PostEndpoints
{
  const string GetPostEndPointName = "getPost";

  public static RouteGroupBuilder MapPostEndPoints(this WebApplication app)
  {
    var group = app.MapGroup("posts").WithParameterValidation();

    group.MapGet("/", (BlogDbContext dbContext) =>
        dbContext.Posts
        .Select(post => post.ToPostSummaryDto())
        .AsNoTracking()
    );

    group.MapGet("/{id}", (int id, BlogDbContext dbContext) =>
   {
     Post? post = dbContext.Posts.Find(id);
     return post is null ? Results.NotFound() : Results.Ok(post.ToPostSummaryDto());

   }).WithName(GetPostEndPointName);

    group.MapPost("/", (PostCreateDto newPost, BlogDbContext dbContext) =>
    {
      Post post = newPost.ToEntity();

      dbContext.Posts.Add(post);
      try
      {
        dbContext.SaveChanges();
      }
      catch (DbUpdateException ex)
      {
        return Results.Problem($"An error occurred while saving the user: {ex.InnerException?.Message ?? ex.Message}");
      }

      return Results.CreatedAtRoute(
         GetPostEndPointName,
         new { id = post.Id },
         post.ToPostSummaryDto());
    }
    );

    group.MapPatch("/{id}", (int id, PostUpdateDto updatePost, BlogDbContext dbContext) =>
   {
     var post = dbContext.Posts.Find(id);
     if (post == null)
     {
       return Results.NotFound($"Post with ID {id} not found.");
     }

     if (!string.IsNullOrWhiteSpace(updatePost.Title))
     {
       post.Title = updatePost.Title;
     }

     if (!string.IsNullOrWhiteSpace(updatePost.Content))
     {
       post.Content = updatePost.Content;
     }

     try
     {
       dbContext.SaveChanges();
     }
     catch (DbUpdateException ex)
     {
       return Results.Problem($"An error occurred while saving the user: {ex.InnerException?.Message ?? ex.Message}");
     }

     return Results.Ok(post.ToPostSummaryDto());
   }
   );

    return group;
  }


}
