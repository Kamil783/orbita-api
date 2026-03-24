namespace Orbita.Application.Models.Dto;

public record AdminUserData(
    Guid UserId,
    string Name,
    string Email,
    string Role,
    byte[]? Avatar,
    DateTime? CreatedAt);
