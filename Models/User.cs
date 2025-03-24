using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BlogPostAPI.Models
{
  [Index(nameof(Email), IsUnique = true)] // Ensure Email is unique
  public class User
  {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name must be at most 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, ErrorMessage = "Username must be at most 50 characters.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool ByPassUserName { get; set; } = false;

    // One-to-Many Relationship (One User → Many Posts)
    public virtual List<Post> Posts { get; set; } = new List<Post>();

    [Column(TypeName = "nvarchar(max)")]
    public string NameHistory { get; set; } = "[]";

    [NotMapped]
    public List<Dictionary<string, string>> NameHistoryList
    {
      get => string.IsNullOrWhiteSpace(NameHistory)
          ? new List<Dictionary<string, string>>()
          : JsonSerializer.Deserialize<List<Dictionary<string, string>>>(NameHistory, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Dictionary<string, string>>();

      set => NameHistory = JsonSerializer.Serialize(value ?? new List<Dictionary<string, string>>());
    }
  }
}
