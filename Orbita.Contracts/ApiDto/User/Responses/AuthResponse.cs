namespace Orbita.Contracts.ApiDto.User.Responses;

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
