using System;

namespace BlogPostApi.DTOs;

public record class UserResponseDto(
  int Id,
  string Name,
  string UserName,
  string Email,
  DateTime CreatedAt
);
