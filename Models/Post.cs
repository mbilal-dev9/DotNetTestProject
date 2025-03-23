using System;
using System.ComponentModel.DataAnnotations;

namespace BlogPostAPI.Models
{
  public class Post
  {
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    // Foreign Key
    public int UserId { get; set; }
    public User? User { get; set; }  // Navigation Property
  }
}
