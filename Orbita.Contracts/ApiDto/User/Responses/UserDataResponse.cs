namespace Orbita.Contracts.ApiDto.User.Responses;

public sealed class UserDataResponse
{
    public string? Name { get; set; }
    public string Email { get; set; } = null!;
    public byte[] Avatar { get; set; } = [];
}
