namespace BlogPostApi.DTOs;

public record class PostResponseDto
(
    int Id,
    string Title,
    string Content,
    int UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
