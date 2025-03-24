using System.ComponentModel.DataAnnotations;

namespace BlogPostApi.DTOs;

public record class UpdateUserDto
(
     [Required]
    [StringLength(100)]
   string Name,

    [StringLength(100)]
   string UserName

);