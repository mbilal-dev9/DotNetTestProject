using System.ComponentModel.DataAnnotations;

namespace BlogPostApi.DTOs;

public record class UserCreateDto(

    [Required]
    [StringLength(100)]
   string Name,

    [StringLength(100)]
   string UserName,

    [Required]
    [EmailAddress]
    [StringLength(150)]
   string Email,

    [Required]
   string Password,

   bool ByPassUserName,

   int? Role

);
