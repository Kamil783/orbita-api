namespace Orbita.Application.Models.Dto;

public record AuthUserDto(Guid UserId, string Email, IReadOnlyList<string> Roles);
