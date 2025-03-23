namespace BlogPostApi.DTOs;

public record class PostUpdateDto(
  string? Title,
  string? Content
);
