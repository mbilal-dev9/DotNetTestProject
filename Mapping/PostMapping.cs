using System;
using BlogPostApi.DTOs;
using BlogPostAPI.Models;

namespace BlogPostApi.Mapping;

public static class PostMapping
{
      public static Post ToEntity(this PostCreateDto post)
    {
      return new Post () {
        UserId = post.UserId,
        Title = post.Title,
        Content = post.Content,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
      };
    } 

    public static PostResponseDto ToPostSummaryDto(this Post post)
  {
    return new(
      post.Id,
      post.Title,
      post.Content,
      post.UserId,
      post.CreatedAt,
      post.UpdatedAt
    );
  }

}
