namespace Orbita.Contracts.ApiDto.User.Requests;

public sealed class RefreshRequest
{
    public string RefreshToken { get; set; } = null!;
}
