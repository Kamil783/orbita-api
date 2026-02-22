namespace Orbita.Contracts.ApiDto.User.Requests;

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
