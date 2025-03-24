using System;

namespace BlogPostApi.DTOs;

public record class UserResponseDto(
  int Id,
  string Name,
  string UserName,
  string Email,
  DateTime CreatedAt,
  DateTime UpdatedAt,
  List<Dictionary<string, string>> NameHistory // Change from string to List<Dictionary<string, string>>
);
