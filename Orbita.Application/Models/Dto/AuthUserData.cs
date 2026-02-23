namespace Orbita.Application.Models.Dto;

public record AuthUserData(Guid UserId, string Email, IReadOnlyList<string> Roles);
