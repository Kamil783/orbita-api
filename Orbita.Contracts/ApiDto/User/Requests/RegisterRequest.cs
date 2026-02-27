namespace Orbita.Contracts.ApiDto.User.Requests;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
