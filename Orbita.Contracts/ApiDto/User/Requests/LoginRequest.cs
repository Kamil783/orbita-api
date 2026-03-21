namespace Orbita.Contracts.ApiDto.User.Requests;

public sealed class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
