namespace Orbita.Application.Models.Dto;

public record RefreshTokenData(string Token, Guid UserId, DateTime ExpiresAt, bool IsRevoked);
