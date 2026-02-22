namespace Orbita.Contracts.ApiDto.User.Requests;

public class RegisterDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
