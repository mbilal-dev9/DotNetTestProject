using BlogPostApi.DTOs;
using BlogPostAPI.Models;

namespace BlogPostApi.Mapping;

public static class UserMapping
{
    public static User ToEntity(this UserCreateDto user)
    {
      return new User () {
        Name = user.Name,
        Email = user.Email,
        UserName = user.UserName ?? "",
        Password = BCrypt.Net.BCrypt.HashPassword(user.Password), // Hash password
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        ByPassUserName = user.ByPassUserName,
        Role = (UserType)(user.Role ?? (int)UserType.User)
      };
    } 

    public static UserResponseDto ToUserSummaryDto(this User user)
  {
    return new(
      user.Id,
      user.Name,
      user.UserName,
      user.Email,
      user.CreatedAt,
      user.UpdatedAt,
      user.NameHistoryList,
      user.IsDeleted,
      ((UserType)user.Role).ToString() 
    );
  }

}
