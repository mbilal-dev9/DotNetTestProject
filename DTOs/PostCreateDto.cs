using System.ComponentModel.DataAnnotations;

namespace BlogPostApi.DTOs;

public record class PostCreateDto
(
  [Required(ErrorMessage = "Title is required.")]
  [StringLength(200, ErrorMessage = "Title must be at most 200 characters.")]
  string Title,

  [Required(ErrorMessage = "Content is required.")]
  string Content,

  [Required(ErrorMessage = "UserId is required.")]
  int UserId
);
