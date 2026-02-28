namespace Orbita.Application.Models.Dto;

public record UserData(Guid UserId, string Email, string Name, byte[] Avatar);
